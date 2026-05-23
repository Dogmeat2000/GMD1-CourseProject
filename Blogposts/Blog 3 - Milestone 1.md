# Milestone 1: The Basics

## Introduction
Milestone 1 focuses on core mechanical minimums and a scalable technical foundation for the project.

## Asset Creation & Prefabs
I established an asset pipeline transitioning from AI-assisted concept art to game-ready Unity models.

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

## Basic Shooting Mechanics
The primary gameplay loop currently features 3D aim tracking, frame-independent rotation, and pooled projectile instantiation.


https://github.com/user-attachments/assets/6b8375ce-b796-4bab-8500-7ff9d60660a1


<br>

A Screen Space Overlay Canvas forms the HUD. To account for 3D depth, Raycasts fire directly from individual barrel transforms, painting dynamic reticles at the actual impact point.

<img src="Blog%203%20-%20PlayerTurretPrefab.jpg" alt="Player Turret Prefab" width="860">

<br>

1. **Player Aims**
    
    Turret rotation and barrel pitch scale based on the active hardware (Mouse Pointer vs. Arcade). Adjustments are multiplied by ```Time.deltaTime``` to ensure uniform rotation speeds across frame rates.

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

    To eliminate camera jitter and reticle desync, camera tracking and HUD updates are handled in ```LateUpdate```, executing only after physics and motor rotations resolve.

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

    The aiming reticle attached to each barrel is updated using ```LateUpdate```, to ensure that these are placed on the screen AFTER the barrel and turret rotations/movements have finished in regular ```Update``` methods.

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

    The motor script passes the fire command along to whatever weapon is attached to it (allowing the for reuseability of this script)

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

    The weapon firing logic utilizes ```delegates``` in combination with the ObjectPool pattern using Unity's built-in ```UnityEngine.Pool``` API. This optimizes performance by reusing projectiles (instead of repeated instantiation and destruction by garbage collector). A fixed number of projectiles are assigned to this weapon (the Pool) and when a projectile is fired, it is taken from the pool - and returns upon destruction

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
