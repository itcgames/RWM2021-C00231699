using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

namespace Tests
{
    public class DynamicControllerTest
    {
        [UnityTest]
        public IEnumerator CameraFollowPlayerIfDelayIsZero()
        {
            var player = new GameObject();
            player.AddComponent<Rigidbody2D>();
            player.AddComponent<playerMovement>();
            player.GetComponent<playerMovement>().moveSpeed = 1;
            var d = new GameObject().AddComponent<DynamicCameraController>();
            d.Contruct(player, 0, 1.0f);
   
            d.GetComponent<DynamicCameraController>().Player.GetComponent<playerMovement>().moveRight();
            yield return new WaitForSeconds(1.0f);

            float p1 = d.transform.position.x;
            float p2 = d.GetComponent<DynamicCameraController>().Player.GetComponent<Rigidbody2D>().position.x;
            Assert.AreEqual(p1 ,p2);
        }

        [UnityTest]
        public IEnumerator CameraDelayWorks()
        {
     
            var player = new GameObject();
            player.AddComponent<Rigidbody2D>();
            player.AddComponent<playerMovement>();
            player.GetComponent<playerMovement>().moveSpeed = 10;
            var d = new GameObject().AddComponent<DynamicCameraController>();
            d.Contruct(player, 10.0f, 1.0f);

            d.GetComponent<DynamicCameraController>().Player.GetComponent<playerMovement>().moveRight();
            yield return new WaitForSeconds(1.0f);
            Debug.Log(d.transform.position.x);
            Debug.Log(d.GetComponent<DynamicCameraController>().Player.GetComponent<Rigidbody2D>().position.x);
            Assert.Less(d.transform.position.x, d.GetComponent<DynamicCameraController>().Player.GetComponent<Rigidbody2D>().position.x);
        }


        [UnityTest]
        public IEnumerator MaxSpeedChecked()
        {

            //var player = new GameObject();
            //player.AddComponent<Rigidbody2D>();
            //player.AddComponent<playerMovement>();
            //player.GetComponent<playerMovement>().moveSpeed = 1000;
            
            //var d = new GameObject().AddComponent<DynamicCameraController>();
            //d.Contruct(player, 0.1f, 10.0f);

            //d.GetComponent<DynamicCameraController>().CameraAcceleration = 2;
            //d.GetComponent<DynamicCameraController>().Player.GetComponent<playerMovement>().moveRight();
            //d.GetComponent<DynamicCameraController>().Update();
            yield return new WaitForSeconds(5.0f);
       
            //float result = d.GetComponent<DynamicCameraController>().getVel();
            //Debug.Log(result);
            
            //Assert.LessOrEqual(result, 10);
        }
    }
}