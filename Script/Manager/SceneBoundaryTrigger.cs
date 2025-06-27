
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class SceneBoundaryTrigger : MonoBehaviour {
    public string targetScene;
    public string entryPointName;
    
    [Header("前跑参数")]
    public float preMoveDistance = 1f;
    public float preMoveSpeed = 3f;
    
    [Header("后跑参数")] 
    public Vector2 exitDirection = Vector2.right; // 新增方向参数
    public float postMoveDistance = 2f;
    public float postMoveSpeed = 3f;

    bool inProgress = false;

    void OnTriggerEnter2D(Collider2D other) {
        if (inProgress || !other.CompareTag("Player")) return;
        StartCoroutine(PreMoveAndTransition(other.GetComponent<PlayerController>()));
    }

    IEnumerator PreMoveAndTransition(PlayerController pc) {
        inProgress = true;
        pc.ignoreInput = true;

        // 前跑方向计算
        Vector3 dir3 = (transform.position - pc.transform.position).normalized;
        Vector2 moveDir = new Vector2(dir3.x, dir3.y);

        // 前跑逻辑
        pc.forcedDir = moveDir.normalized;
        pc.forcedRunSpeed = preMoveSpeed;
        pc.forcedRun = true;

        float remain = preMoveDistance;
        while (remain > 0.01f) {
            remain -= preMoveSpeed * Time.deltaTime;
            yield return null;
        }

        // 触发场景切换，传递后跑参数
        SceneTransitionManager.Instance.TransitionTo(
            targetScene,
            entryPointName,
            exitDirection,    // 使用预设的出口方向
            postMoveDistance,
            postMoveSpeed
        );
        
        inProgress = false;
    }
}