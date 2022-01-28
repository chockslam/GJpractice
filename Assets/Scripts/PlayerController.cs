using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] public float speed = 1; // speed of the player
    [SerializeField] public float JumpPower = 0.5f; // speed of the player
    [SerializeField] private LayerMask groundLayer; // ground layer variable that facilitates raycasting that facilitate collision detection on jump
    [SerializeField] private LayerMask WallLayer; // Wall layer variable that facilitates raycasting that facilitate collision detection on wall jump
    private Rigidbody2D body; // physics body of the palyer
    private Animator anim; // animator facilitates animations. mainly used to traverse states in state machine
    private BoxCollider2D boxCollider; // Box collider of the player. Facilitates collision detection.
    private float wallJumpCooldown;
    float horizInput;
    // On creation
    private void Awake() {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    // Every frame
    private void Update() {

        horizInput = Input.GetAxis("Horizontal"); // get horizontal axis input 

      
        // Facilitates *flipping* of the sprite
        if(horizInput>0.01f)
            transform.localScale = new Vector3(5,5,5);
        else if(horizInput<-0.01f)
            transform.localScale = new Vector3(-5,5,5);
        
        

        // used to facilitate state transision
        anim.SetBool("walk", horizInput !=0); // walk if x axis input is not equals to zero
        anim.SetBool("grounded", isGrounded()); // grounded...

        if(wallJumpCooldown > 0.2f){
            
                
            body.velocity = new Vector2(horizInput * speed,body.velocity.y); // facilitate movement right and left (D and A)
            if(onWall() && !isGrounded()){
                body.gravityScale = 0;
                body.velocity = Vector2.zero;
            }else{
                body.gravityScale = 2;
            }

            //Jump if is grounded and space is pressend
            if(Input.GetKey(KeyCode.Space) )
                Jump();
        }
        else
            wallJumpCooldown += Time.deltaTime;

    }
    
private void Jump(){
    if(isGrounded()){
        body.velocity = new Vector2(body.velocity.x, JumpPower);// realistic physsics jumping , depends on the speed and velocity on x axis
        anim.SetTrigger("jump"); // trigger jumping state
        // grounded = false;
    }
    else if(onWall() && !isGrounded()){


        if(horizInput == 0){
            body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 10, 0);
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        else
        body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 3,5);

        wallJumpCooldown = 0;
    }
}



//Returns whether the player is grounded using boxcasting
private bool isGrounded(){
    // returns whether the box collider that points down hits an object with the tag stored in serialized GroundLayer var
    RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
    // return whether the boxCollider actually hits the groundLayer. So returns true, if it is not equal to zero.
    return raycastHit.collider != null;
    
}

// Jumping from walls
private bool onWall(){
    // returns whether the box collider that points down hits an object with the tag stored in serialized GroundLayer var
    RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, WallLayer);
    // return whether the boxCollider actually hits the groundLayer. So returns true, if it is not equal to zero.
    return raycastHit.collider != null;
    
}

}

