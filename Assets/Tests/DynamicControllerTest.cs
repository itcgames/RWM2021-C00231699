using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

namespace Tests
{
    public class DynamicControllerTest
    {
        [UnityTest]
        public IEnumerator CameraFollowPlayerIfLazyLoad()
        {
            var player = new GameObject();
            player.AddComponent<Rigidbody2D>();
            player.AddComponent<playerMovement>();
            player.GetComponent<playerMovement>().moveSpeed = 1;
            var d = new GameObject().AddComponent<DynamicCameraController>();
            d.Contruct(player);
            d.useLazyCamera = true;
            d.followSpeedPercentage = 1;
            float p3 = d.transform.position.x;
            d.GetComponent<DynamicCameraController>().Player.GetComponent<playerMovement>().moveRight();
            yield return new WaitForSeconds(1.0f);

            float p1 = d.transform.position.x;
            float p2 = d.GetComponent<DynamicCameraController>().Player.GetComponent<Rigidbody2D>().position.x;
            Assert.Less(p1 ,p2);    // Less than player
            Assert.Less(p3, p1);       // Not at the start still
        }

        [UnityTest]
        public IEnumerator CameraStickToPlayerNotLazy()
        {
     
            var player = new GameObject();
            player.AddComponent<Rigidbody2D>();
            player.AddComponent<playerMovement>();
            player.GetComponent<playerMovement>().moveSpeed = 10;
            var d = new GameObject().AddComponent<DynamicCameraController>();
            d.Contruct(player);
            d.useLazyCamera = false;

            d.GetComponent<DynamicCameraController>().Player.GetComponent<playerMovement>().moveRight();
            yield return new WaitForSeconds(1.0f);

            float p1 = d.transform.position.x;
            float p2 = d.GetComponent<DynamicCameraController>().Player.GetComponent<Rigidbody2D>().position.x;
            Assert.AreEqual(p1,p2);
        }


        [UnityTest]
        public IEnumerator LazyCamZeroDoesntFollow()
        {

            var player = new GameObject();
            player.AddComponent<Rigidbody2D>();
            player.AddComponent<playerMovement>();
            player.GetComponent<playerMovement>().moveSpeed = 10;
            var d = new GameObject().AddComponent<DynamicCameraController>();
            d.Contruct(player);
            d.useLazyCamera = true;
            d.followSpeedPercentage = 0;
            float p3 = d.transform.position.x;
            d.GetComponent<DynamicCameraController>().Player.GetComponent<playerMovement>().moveRight();
            yield return new WaitForSeconds(1.0f);

            float p1 = d.transform.position.x;
            float p2 = d.GetComponent<DynamicCameraController>().Player.GetComponent<Rigidbody2D>().position.x;
            Assert.Less(p1, p2);    // Less than player
            Assert.AreEqual(p3, p1);       // Is at the start still
        }

        [UnityTest]
        public IEnumerator LazyCamFullSticksToPlayer()
        {

            var player = new GameObject();
            player.AddComponent<Rigidbody2D>();
            player.AddComponent<playerMovement>();
            player.GetComponent<playerMovement>().moveSpeed = 10;
            var d = new GameObject().AddComponent<DynamicCameraController>();
            d.Contruct(player);
            d.useLazyCamera = true;
            d.followSpeedPercentage = 100;
            float p3 = d.transform.position.x;
            d.GetComponent<DynamicCameraController>().Player.GetComponent<playerMovement>().moveRight();
            yield return new WaitForSeconds(1.0f);

            float p1 = d.transform.position.x;
            float p2 = d.GetComponent<DynamicCameraController>().Player.GetComponent<Rigidbody2D>().position.x;
            Assert.AreEqual(p1, p2);    // Same than player
        }

        [UnityTest]
        public IEnumerator BoundaryValuesWork()
        {
            var player = new GameObject();
            player.AddComponent<Rigidbody2D>();
            player.AddComponent<playerMovement>();
            player.GetComponent<playerMovement>().moveSpeed = 100;
            var d = new GameObject().AddComponent<DynamicCameraController>();
            d.Contruct(player);
            d.useLazyCamera = false;
            d.useBoundaryPositions = true;
            d.boundaryBottomRight.x = 2;
            d.boundaryBottomRight.y = -10;

            d.GetComponent<DynamicCameraController>().Player.GetComponent<playerMovement>().moveRight();
            yield return new WaitForSeconds(2.0f);

            float p1 = d.transform.position.x;
  
            Assert.AreEqual(p1, 2);    
        }

        [UnityTest]
        public IEnumerator ShakeRotateWorks()
        {
            var player = new GameObject();
            player.AddComponent<Rigidbody2D>();
            player.AddComponent<playerMovement>();
            player.GetComponent<playerMovement>().moveSpeed = 100;
            var d = new GameObject().AddComponent<DynamicCameraController>();
            d.Contruct(player);
            d.useLazyCamera = false;
            d.useCameraShake = true;
            d.trauma = 1;
            d.traumaReductionRate = 0;
            d.maxAngleOffset = 1;
            Vector3 startAngle = d.transform.eulerAngles;

            yield return new WaitForSeconds(1.0f);

            Vector3 afterAngle = d.transform.eulerAngles;

            Assert.AreNotEqual(startAngle.z, afterAngle.z);
        }

        [UnityTest]
        public IEnumerator ShakeTranslateWorks()
        {
            var player = new GameObject();
            player.AddComponent<Rigidbody2D>();
            player.AddComponent<playerMovement>();
            player.GetComponent<playerMovement>().moveSpeed = 100;
            var d = new GameObject().AddComponent<DynamicCameraController>();
            d.Contruct(player);
            d.useLazyCamera = false;
            d.useCameraShake = true;
            d.trauma = 1;
            d.traumaReductionRate = 0;
            d.maxPositionOffset = 1;
            Vector3 posBF = d.transform.position;

            yield return new WaitForSeconds(1.0f);

            Vector3 posAF = d.transform.position;

            Assert.AreNotEqual(posAF.x, posBF.x);
        }

        [UnityTest]
        public IEnumerator ShakeStoppingWorks()
        {
            var player = new GameObject();
            player.AddComponent<Rigidbody2D>();
            player.AddComponent<playerMovement>();
            player.GetComponent<playerMovement>().moveSpeed = 100;
            var d = new GameObject().AddComponent<DynamicCameraController>();
            d.Contruct(player);
            d.useLazyCamera = false;
            d.useCameraShake = true;
            d.trauma = 1;
            d.traumaReductionRate = 1;
            d.maxPositionOffset = 1;

            Vector3 posBF = d.transform.position;

            yield return new WaitForSeconds(1.0f);

            Vector3 posAF = d.transform.position;

            Assert.AreEqual(posAF.x, posBF.x);
        }

        [UnityTest]
        public IEnumerator lockAxisWorks()
        {
            var player = new GameObject();
            player.AddComponent<Rigidbody2D>();
            player.AddComponent<playerMovement>();
            player.GetComponent<playerMovement>().moveSpeed = 100;
            var d = new GameObject().AddComponent<DynamicCameraController>();
            d.Contruct(player);
            d.useLazyCamera = false;
            d.lockYAxis = true;
            
            float posBF = d.transform.position.y;

            d.GetComponent<DynamicCameraController>().Player.GetComponent<playerMovement>().jump();
            yield return new WaitForSeconds(0.1f);

            float posAF = d.transform.position.y;

            Assert.AreNotEqual(posAF, posBF);
        }

        [UnityTest]
        public IEnumerator VelocityAdjustTest()
        {
            var player = new GameObject();
            player.AddComponent<Rigidbody2D>();
            player.AddComponent<playerMovement>();
            player.GetComponent<playerMovement>().moveSpeed = 100;
            var d = new GameObject().AddComponent<DynamicCameraController>();
            d.Contruct(player);
            d.useLazyCamera = false;
            d.velocityAdjust = true;
            d.minVelocityAdjust = 10;

            d.GetComponent<DynamicCameraController>().Player.GetComponent<playerMovement>().moveRight();
            yield return new WaitForSeconds(0.1f);

            float posBF = d.transform.position.x;
            float posAF = d.GetComponent<DynamicCameraController>().Player.GetComponent<Rigidbody2D>().position.x;

            Assert.Less(posAF, posBF); // Player less than camera
        }
    }
}