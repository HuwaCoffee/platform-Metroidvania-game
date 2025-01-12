using UnityEngine;
using UnityEngine.EventSystems;

public class InitButton : MonoBehaviour
{
    private GameObject lastSelect;

    void Start()
    {
        lastSelect = new GameObject(); // ��ʼ��һ���ն���
    }

    void Update()
    {
        var currentSelected = EventSystem.current.currentSelectedGameObject;

        if (currentSelected == null) // �����ǰû��ѡ����
        {
            EventSystem.current.SetSelectedGameObject(lastSelect); // �ָ�����һ��ѡ����
        }
        else if (currentSelected != lastSelect) // �����ǰѡ����ı�
        {
            lastSelect = currentSelected; // �����ϴ�ѡ����
        }
    }

    public void OnPointerEnter(GameObject hoveredObject)
    {
        // �������ͣʱ��������ͣ��ťΪ��ǰѡ����
        EventSystem.current.SetSelectedGameObject(hoveredObject);
    }

    public void OnPointerExit(GameObject exitedObject)
    {
        // ������Ƴ�ʱ���ָ����ϴε�ѡ��������ѡ�У�
        if (EventSystem.current.currentSelectedGameObject == exitedObject)
        {
            EventSystem.current.SetSelectedGameObject(lastSelect);
        }
    }
}
    