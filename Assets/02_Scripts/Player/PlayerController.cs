using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Camera")]
    public Camera mainCam;

    [Header("Move")]
    public float moveSpeed = 4f;
    public LayerMask isGround;

    [Header("Life Setting")]
    public float Hp = 10;
    public bool isDead;

    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (isDead) return;

        //화살표 입력
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 inputDir = new Vector3(h, 0, v);

        //카메라 방향
        var camForward = mainCam.transform.forward;
        var camRight = mainCam.transform.right;
        camForward.y = 0;
        camRight.y = 0;

        Vector3 desireDir = camForward * inputDir.z + camRight * inputDir.x;

        MovePlayer(desireDir);
        TurnPlayer();
    }
    //캐릭터 이동
    void MovePlayer(Vector3 _desireDir)
    {
        Vector3 movement = new Vector3(_desireDir.x, 0, _desireDir.z);
        movement = movement.normalized * moveSpeed * Time.deltaTime;

        rb.MovePosition(transform.position + movement);
    }
    //캐릭터회전
    void TurnPlayer()
    {
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, isGround))
        {
            Vector3 playerToMouse = hit.point - transform.position;
            playerToMouse.y = 0;
            playerToMouse.Normalize();

            Quaternion newRotation = Quaternion.LookRotation(playerToMouse);
            rb.MoveRotation(newRotation);
        }
    }

    //죽었는지 판정
    public void Died()
    {
        if (isDead) return;

        isDead = true;

        this.gameObject.SetActive(false);
    }
}
