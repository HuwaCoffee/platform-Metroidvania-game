using System.Collections;
using System.Collections.Generic;
using UnityEngine;




// }
public class CheckPoint : MonoBehaviour
{
    [SerializeField] private float saveRadius = 2f;
    //[SerializeField] private GameObject saveEffect;这里以后打算做成渐入和渐出

    public PlayerController pc;
   
    // 新增E键提示相关变量
    [Header("E键提示设置")]
    public SpriteRenderer eKeySprite; // 拖入E键提示的SpriteRenderer组件
    public float fadeDuration = 0.5f; // 淡入淡出持续时间
    public Vector3 iconOffset = new Vector3(0, 1.5f, 0); // 图标相对NPC的偏移量（头顶位置）
    private Coroutine fadeCoroutine; // 淡入淡出协程引用
    private float targetAlpha; // 目标透明度

void Start(){
     // 初始化E键提示为完全透明
        if (eKeySprite != null)
        {
            eKeySprite.color = new Color(eKeySprite.color.r, eKeySprite.color.g, eKeySprite.color.b, 0f);
        }
}
    private void Update()
    {
        // 更新E键图标位置（始终位于NPC头顶）
        if (eKeySprite != null)
        {
            eKeySprite.transform.position = transform.position + iconOffset;
            // 可选：始终面向玩家（2D情况可省略）
            //eKeySprite.transform.LookAt(playerTransform);
        }
        bool isPlayerN;
        if(Vector3.Distance(transform.position,
            PlayerController.Instance.transform.position) < saveRadius){
                isPlayerN=true;
            }else{
                isPlayerN=false;
            }
        bool isPlayerInRange = isPlayerN ;
        
        // 设置目标透明度
        targetAlpha = isPlayerInRange ? 1f : 0f;

        // 检测透明度变化需求
        if (Mathf.Abs(eKeySprite.color.a - targetAlpha) > 0.01f)
        {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeEKey());
        }

        if (Vector3.Distance(transform.position,
            PlayerController.Instance.transform.position) < saveRadius
            && Input.GetKeyDown(KeyCode.E))
        {
            PerformSave();
        }
    }
 // 淡入淡出协程（直接操作SpriteRenderer的颜色）
    private IEnumerator FadeEKey()
    {
        Color currentColor = eKeySprite.color;
        float startAlpha = currentColor.a;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / fadeDuration);
            currentColor.a = Mathf.Lerp(startAlpha, targetAlpha, t);
            eKeySprite.color = currentColor;
            yield return null;
        }

        // 确保最终值准确
        currentColor.a = targetAlpha;
        eKeySprite.color = currentColor;
        fadeCoroutine = null;
    }

    private void PerformSave()
    {
        // 视觉反馈
        
        GameController.camShake.Zoom();//渐入和渐出

        // 保存游戏
        BinarySaveManager.SaveGame();

        // 刷新敌人
        //EnemyPoolManager.Instance.ResetAllEnemies();
        BinarySaveManager.LoadGame();

        // 恢复血量
        HealthBar.HealthCurrent = HealthBar.HealthMax;
    }
}