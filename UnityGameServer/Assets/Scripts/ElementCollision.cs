using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementCollision : MonoBehaviour
{
    private bool isColliding = false;
    public Player player;
    public GameObject pointingArrow;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "NotRoad" && !player.arrowActive && player.lastGlassRef)
        {
            PacketSend.ActivatePoitingArrowAndSendMessage(player.id, player.lastGlassRef.transform.position, player.lastGlassRef.transform.rotation, "Regresa a la pista");
            //alert.SetActive(true);
            //alert.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Regresa a la pista";
            player.arrowActive = true;
            ShowPointingArrow();
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "NotRoad")
        {
            PacketSend.ShowAlertWithMessage(player.id, "Regresa a la pista", true);
            //alert.SetActive(true);
            //alert.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Regresa a la pista";
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "NotRoad")
        {

            PacketSend.ShowAlertWithMessage(player.id, "Estás avanzando en sentido contrario", false);
            //alert.SetActive(false);
            //alert.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Estás avanzando en sentido contrario";
        }
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        if (isColliding) return;
    //        Debug.Log("HITT!");
    //        isColliding = true;
    //        StartCoroutine(Reset());

    //        Player player = other.transform.gameObject.GetComponent<Player>();

    //        player.speed *= 0.80f;
    //        player.collisions += 1;

    //        if (tag == "Tires")
    //        {
    //            PacketSend.ElementCollision("Tires", player, this);
    //        }
    //        else if (tag == "Rock")
    //        {
    //            PacketSend.ElementCollision("Rock", player, this);
    //        }
    //        else if (tag == "Tree")
    //        {
    //            PacketSend.ElementCollision("Tree", player, this);
    //        }
    //    }
    //}

    //IEnumerator Reset()
    //{
    //    yield return new WaitForSeconds(3);
    //    isColliding = false;
    //}

    void ShowPointingArrow()
    {
        player.ptArrow = Instantiate(pointingArrow, new Vector3(player.lastGlassRef.transform.position.x,
        -8f, player.lastGlassRef.transform.position.z),
        Quaternion.identity);
    }
}
