using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallTrigger : MonoBehaviour
{
    [SerializeField] Transform returnPoint;
    [SerializeField] GameObject ball;
    [SerializeField] float BallAngle;
    [SerializeField] float BallPower;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject == ball)
        {
            ball.transform.position = returnPoint.transform.position;
            Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();
            rb.velocity = Vector2.zero;
            float ogGrav = rb.gravityScale;
            rb.gravityScale = 0;
            Timer.SimpleTimer(() =>
            {
                rb.AddForce(new Vector2(Mathf.Cos(BallAngle * Mathf.Deg2Rad) * (Random.Range(0, 2) > 0 ? -1 : 1), Mathf.Sin(BallAngle * Mathf.Deg2Rad)) * BallPower, ForceMode2D.Impulse);
                rb.gravityScale = ogGrav;
            }, 1);
        }
    }
}
