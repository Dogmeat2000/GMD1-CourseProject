# Milestone 1: The Basics

## Introduction
Milestone 1 focuses on implementing core mechanical minimums and establishing a scalable technical foundation for the project.

## Asset Creation & Prefabs
I established a streamlined asset pipeline transitioning from AI-assisted concept art to game-ready Unity models.

<img src="Blog%203%20-%20Asset%20Creation%20Pipeline.gif" alt="Pipeline" width="860">

<br>

1. **Concept art:** <br>Generated via local inference models (Flux2.dev, Z_ImageTurbo, Qwen-Image). Neutral backgrounds are enforced to optimize downstream extraction.<br><br>
2. **Multi perspective Orthographic Renders:**<br>Multi-perspective views are utilized to feed 3D generators, yielding significantly higher mesh fidelity than single-reference inputs.<br><br>
3. **High Poly Mesh:**<br>Generated using Trellis.2 (Microsoft Open Source) and TripoAI (commercial). TripoAI currently provides superior automated texturing.<br><br>
4. **Game Ready Meshes:**<br>Meshes are manually voxelized for watertightness, retopologized, and texture-baked in Blender, achieving >99.5% polygon reduction with minimal detail loss. I manually create three LODs (`_LOD0`, `_LOD1`, `_LOD2`) for granular rendering control upon Unity import.
<img src="Blog%203%20-%20Mesh%20Import.jpg" alt="Asset Import in Unity" width="1080">

<br>

**Using Prefabs**<br>
Imported meshes are converted into modular Prefabs (prefixed `PFB_`). This encapsulates configuration and behavior for rapid reuse. All Prefabs are calibrated in an isolated sandbox scene (`SCN_TestBench`) prior to live deployment.

<img src="Blog%203%20-%20Prefabs.jpg" alt="Prefabs in Unity" width="860">

<br>

## Project Architecture, Basic Scenes & UI
To maintain structural integrity as the project scales, I implemented a strict, modular directory architecture:
``` 
00_Scenes       -> Holds all scenes

01_Scripts      -> Holds all Scripts, in structure subfolders, categorized by object types.
   - Core       -> Scripts that are shared between object types (i.e. shooting mechanics, health mechanics, etc.)
   - Ships
   - Turrets
   - etc.

02_Prefabs      -> Holds all prefab objects, that are ready for use in game.
   - Enemies
   - PlayerTurrets
   - Projectiles
   - Ships
   - UI
   - Etc.

03_Art          -> Holds everything related to raw Art and Design.
   - Animations -> Animations used by game objects.
   - Materials  -> Materials used by game objects.
   - Models     -> Models imported.
   - Textures   -> Textures used in Materials and UI elements.

04_Audio        -> Holds everything related to Audio.
   - Music
   - SFX

05_Settings     -> Holds everything related to settings, such as the Unity Input System and input configuration.
   - Input

99_ThirdParty   -> Holds imported third party assets, such as fonts or samples (i.e. the VIA Arcade Machine sample)

StreamingAssets -> Holds all assets that must be streamed (i.e. main menu background video), to maintain compatibility with the WebGL Build and deployments to GitHub Pages.
```
This is an initial structure, that I plan to expand as the game develops and architectural requirements become more clear, but as an initial starting point I expect this to provide a good basis for keeping the game project structured, and thus allow me to save time during future development.

<br>

**Naming Standard:** To better improve my overall project overview, as well as the ease of searching for specific asset types, I have decided to use this basic naming conventions for Unity assets. This makes it immediately visible what type each asset is - and drastically improves searching for a specific asset type in a larger project using the Unity search tools.
``` 
SCN_        : Scenes
PFB_        : Prefabs
MAT_        : Materials
SK_         : Models that contain a Skeleton (for organic object animation)
SM_         : Static Meshes (i.e. static non-animated objects)
T_..._BC    : Texture, of the Base Color type.
T_..._N     : Texture, of the Normal Map type.
T_..._Mask  : Texture, of the Mask Type, which contains Metallic, Roughness and Ambient Occlusion maps.
RT_         : Render-Textures. Useful for rendering videos.
Music_      : Self-explanatory...
SFX_        : Special Effects (such as cannon fire sound)
```
I expect more will be added as the game development progresses.

## Basic Shooting Mechanics
The primary gameplay loop currently features 3D aim tracking, frame-independent rotation, and pooled projectile instantiation.

<video src="https://github.com/Dogmeat2000/GMD1-CourseProject/raw/refs/heads/main/Blogposts/Blog%203%20-%20Video%20of%20Basic%20Shooting%20Mechanics.mp4?download=" controls width="860">
  Your browser does not support the video tag.
</video>

<br>

A Screen Space Overlay Canvas forms the HUD. To account for 3D depth, Raycasts fire directly from individual barrel transforms, painting dynamic reticles accurately at the actual impact point.

<img src="Blog%203%20-%20PlayerTurretPrefab.jpg" alt="Player Turret Prefab" width="860">

<br>

1. **Player Aims**
    
    Turret rotation and barrel pitch scale dynamically based on the active hardware (Mouse Pointer vs. Arcade). Adjustments are multiplied by ```Time.deltaTime``` to ensure uniform rotation speeds across varying frame rates.
    ```csharp
    [RequireComponent(typeof(TurretMotor))]
    public class TurretPlayerInput : MonoBehaviour
    {
        ///...

        void Update() {
            Vector2 input = moveAction.action.ReadValue<Vector2>();
            
            bool isMouse = moveAction.action.activeControl?.device is Pointer;
            float sens = isMouse ? mouseSens : arcadeSens;
            
            _motor.RotateJoints(input.x * sens * Time.deltaTime, input.y * sens * Time.deltaTime);
            
            if (_isFireRequested) {
                if (EventSystem.current && !EventSystem.current.IsPointerOverGameObject())
                {
                    _motor.PullTrigger();
                }
                _isFireRequested = false;
            }
        }
    }
    ```

    The ```TurretMotor``` processes and executes these translated physical limits:
    ```csharp
    public class TurretMotor : MonoBehaviour
    {
        ///...

        public void RotateJoints(float yawDelta, float pitchDelta) {
            if (barrelBase) {
                _currentPitch += pitchDelta;
                _currentPitch = Mathf.Clamp(_currentPitch, minPitch, maxPitch);
                barrelBase.localRotation = Quaternion.Euler(0f, _currentPitch, 0f);
            }

            if (turretBase) {
                turretBase.Rotate(Vector3.forward * yawDelta, Space.Self);
            }
        }
    }
    ```

    To eliminate camera jitter and reticle desync, camera tracking and HUD updates are strictly handled in ```LateUpdate```, executing only after physics and motor rotations resolve.

    ```csharp
    public class TurretCameraFollow : MonoBehaviour
    {
        ///...

        private void LateUpdate()
        {
            if (!barrelPivot) 
                return;

            float currentBarrelPitch = barrelPivot.localEulerAngles.y;
            if (currentBarrelPitch > 180) currentBarrelPitch -= 360;
            float trackingOffset = currentBarrelPitch * followFactor;
            transform.localRotation = _initialLocalRotation * Quaternion.Euler(-trackingOffset, 0f, 0f);
        }
    }
    ```

    The aiming reticle attached to each barrel is also updated using ```LateUpdate```, to ensure that these are placed on the screen AFTER the barrel and turret rotations/movements have been finished in the regular ```Update``` methods.

    ```csharp
    public class TurretHUD : MonoBehaviour
    {
        ///...

        private void LateUpdate()
        {
            if (!turretCamera) return;

            UpdateReticlePosition(lowerMuzzleExit, lowerReticleUI);
            UpdateReticlePosition(upperMuzzleExit, upperReticleUI);
        }
    
        private void UpdateReticlePosition(Transform muzzle, RectTransform reticle)
        {
            if (!muzzle || !reticle) return;

            Ray targetRay = new Ray(muzzle.position, muzzle.forward);
            Vector3 worldImpactPoint = Physics.Raycast(targetRay, out RaycastHit hit, 750f) ? hit.point : targetRay.GetPoint(750f);

            Vector3 screenPoint = turretCamera.WorldToScreenPoint(worldImpactPoint);
            
            // z > 0 means the target is in front of the camera
            bool isTargetVisible = screenPoint.z > 0;
        
            reticle.gameObject.SetActive(isTargetVisible);
            if (isTargetVisible)
            {
                reticle.position = screenPoint;
            }
        }

    }
    ```


2. **Player fires**

    Firing logic relies on an event-driven ```InputAction.CallbackContext```. Instead of executing immediately and risking frame-desync, the event sets a boolean flag evaluated safely during the primary ```Update``` cycle, ensuring UI interactions don't trigger accidental discharge.
    ```csharp 
    public class TurretPlayerInput : MonoBehaviour
    {
        //...
        [SerializeField] private InputActionReference fireAction;
        //...

        private TurretMotor _motor;
        private bool _isFireRequested = false;

        //...

        private void ExecuteFireCommand(InputAction.CallbackContext context){
            _isFireRequested = true;
        }

        void Update() {
            //...
            
            if (_isFireRequested)
            {
                if (EventSystem.current && !EventSystem.current.IsPointerOverGameObject())
                {
                    _motor.PullTrigger();
                }
                _isFireRequested = false;
            }
        }
    }
    ```

    The motor script passes the fire command along to whatever weapon is attached to it (allowing the for reuseability of this script). 
    ```csharp
    public class TurretMotor : MonoBehaviour
    {
        //...
        [SerializeField] private TurretWeapon mainWeapon;
        //...

        public void PullTrigger() { 
            // Do not fire, if game is paused
            if (Time.timeScale <= 0f) return;
            
            if (mainWeapon) {
                mainWeapon.Fire(); 
            }
        }
    }
    ```

    The weapon firing logic heavily utilizes ```delegates``` in combination with the ObjectPool pattern using Unity's built-in ```UnityEngine.Pool``` API. This optimizes performance by allowing projectiles to be reused (instead of repeatedly instantiating and then destroying projectiles continiously through the garbage collector). A fixed number of projectiles are assigned to this weapon (the Pool) and when a projectile is fired, it is taken from this pool - and returns to this pool upon impact ("destruction").

    ```csharp
    public class TurretWeapon : MonoBehaviour
    {
        //...
        [SerializeField] private TurretProjectile projectilePrefab;
        //...
    
        [Header("Magazine (Pool) Settings")]
        [SerializeField] private int defaultCapacity = 50;
        [SerializeField] private int maxSize = 200;

        //...

        private IObjectPool<TurretProjectile> _projectilePool;

        //...

        private void Awake()
        {
            _projectilePool = new ObjectPool<TurretProjectile>(
                createFunc: CreateProjectile,
                actionOnGet: OnTakeFromPool,
                actionOnRelease: OnReturnedToPool,
                actionOnDestroy: OnDestroyPoolObject,
                collectionCheck: true,
                defaultCapacity: defaultCapacity,
                maxSize: maxSize
            );
        }

        private TurretProjectile CreateProjectile()
        {
            TurretProjectile projectile = Instantiate(projectilePrefab);
            projectile.SetPool(_projectilePool);
            return projectile;
        }

        private void OnTakeFromPool(TurretProjectile projectile){
            //...
        }

        private void OnReturnedToPool(TurretProjectile projectile){
            //...
        }

        private void OnDestroyPoolObject(TurretProjectile projectile){
            //...
        }

        public void Fire(){
            if (Time.time >= _nextFireTime){
                _nextFireTime = Time.time + fireRate;
                _projectilePool.Get(); 
            
                if (weaponAudioSource && fireSound){
                    weaponAudioSource.PlayOneShot(fireSound);
                }
            }
        }
    }
    ```

    Projectiles self-report back to their assigned pool upon lifetime expiration or collision.

    ```csharp
    public class TurretProjectile : MonoBehaviour
    {
        //...
        private IObjectPool<TurretProjectile> _managedPool;
        //...

        public void SetPool(IObjectPool<TurretProjectile> pool){
            _managedPool = pool;
        }

        //...

        private void ReturnToPool(){
            if (gameObject.activeSelf){
                _managedPool?.Release(this);
            }
        }
    }
    ```


## CI/CD
Continuous Integration and Delivery has been important for me to get up and running early. This allows me to verify game buildability and compatibility with WebGL and Windows-Native environments early on - which I expect will save time later. 

<img src="Blog%203%20-%20CI-CD.jpg" alt="CI-CD" width="860">
