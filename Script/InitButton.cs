using UnityEngine;
using UnityEngine.EventSystems;

public class InitButton : MonoBehaviour
{
    private GameObject lastSelect;

    void Start()
    {
        lastSelect = new GameObject(); // 初始化一个空对象
    }

    void Update()
    {
        var currentSelected = EventSystem.current.currentSelectedGameObject;

        if (currentSelected == null) // 如果当前没有选中项
        {
            EventSystem.current.SetSelectedGameObject(lastSelect); // 恢复到上一次选中项
        }
        else if (currentSelected != lastSelect) // 如果当前选中项改变
        {
            lastSelect = currentSelected; // 更新上次选中项
        }
    }

    public void OnPointerEnter(GameObject hoveredObject)
    {
        // 当鼠标悬停时，设置悬停按钮为当前选中项
        EventSystem.current.SetSelectedGameObject(hoveredObject);
    }

    public void OnPointerExit(GameObject exitedObject)
    {
        // 当鼠标移出时，恢复到上次的选中项（避免空选中）
        if (EventSystem.current.currentSelectedGameObject == exitedObject)
        {
            EventSystem.current.SetSelectedGameObject(lastSelect);
        }
    }
}
    