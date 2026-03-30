using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Animator animator; //动画机
    public PlayerFSM FSM_0; //第一层状态机
    public PlayerFSM FSM_1; //第二层状态机

    public bool IsTryDown => Input.GetKey(KeyCode.S);
    public bool IsTryUp => Input.GetKey(KeyCode.W);
    public bool IsTryJump => Input.GetKey(KeyCode.Space);
    public bool IsTryPunch => Input.GetKey(KeyCode.A);
    public bool IsTryStopPunch => Input.GetKey(KeyCode.D);

    private void OnEnable()
    {
        FSM_0 = new PlayerFSM();
        // FSM_0.AddState(new Player_Idle(this, "Idle"));
        // FSM_0.AddState(new Player_Down(this, "Down"));
        // FSM_0.AddState(new Player_Down_Idle(this, "Down_Idle"));
        // FSM_0.AddState(new Player_Up(this, "Up"));
        // FSM_0.AddState(new Player_Jumping(this, "Jumping"));
        //
        // FSM_1 = new PlayerFSM();
        // FSM_1.AddState(new Player_DoNothing(this, "DoNothing"));
        // FSM_1.AddState(new Player_Punch(this, "Punching"));
    }

    private void Start()
    {
        // FSM_0.SwitchOn(typeof(Player_Idle));
        // FSM_1.SwitchOn(typeof(Player_DoNothing));
    }

    private void Update()
    {
        FSM_0.OnUpdate();
        FSM_1.OnUpdate();
    }
}