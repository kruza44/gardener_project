using UnityEngine;

public class PreventPush : MonoBehaviour
{
	private Vector3 targetDirection;
	private Rigidbody2D rb; //상대방의 Rigidbody

	void OnTriggerEnter2D(Collider2D other)
	{
		rb = other.transform.root.GetComponent<Rigidbody2D>();
	}

	void OnTriggerStay2D(Collider2D other)
	{
        Rigidbody2D myrb;
        myrb = transform.root.GetComponent<Rigidbody2D>();
        Debug.Log(myrb.velocity);
		targetDirection = transform.position - other.transform.position;
		Vector2 otherSpeed = rb.velocity;

        Debug.Log(otherSpeed);

		otherSpeed = Vector3.Project(otherSpeed, targetDirection); //속도의 자신 <- 상대방 방향
		rb.velocity -= otherSpeed; // 자신 <- 상대방 방향의 속도만 0으로 만든다
	}
}

