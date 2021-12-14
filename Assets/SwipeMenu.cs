#define OLD1

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SwipeMenu : MonoBehaviour
{
    [SerializeField]
    Scrollbar scrollbar;

    float[] positions;
    float halfDistance;
    float scrollPos = 0;
#if !OLD
    Coroutine coroutine;
    const float delta = 0.01f;
#endif

    void Awake()
    {
        positions = new float[transform.childCount];
        float distance = 1f / (positions.Length - 1f);
        for (int i = 0; i < positions.Length; i++)
        {
            positions[i] = distance * i;
        }

        halfDistance = distance / 2;
    }

    void Start()
    {
#if !OLD
        for (int i = 1; i < positions.Length; i++)
        {
            transform.GetChild(i).localScale = new Vector3(0.8f, 0.8f, 0.8f);
        }
#endif
    }

    void Update()
    {
#if OLD
        int index = GetActualPostionIndex();
        if (Input.GetMouseButton(0))
        {
            scrollPos = scrollbar.value;
        }
        else
        {
            scrollbar.value = Mathf.Lerp(scrollbar.value, positions[index], 3f * Time.deltaTime);
        }

        for (int i = 0; i < positions.Length; i++)
        {
            float scale = i == index ? 1 : 0.8f;
            transform.GetChild(i).localScale = Vector2.Lerp(transform.GetChild(i).localScale, new Vector2(scale, scale), 3f * Time.deltaTime);
        }
#endif
    }

#if !OLD
    public void OnChange(Vector2 value)
    {
        if (coroutine == null)
        {
            coroutine = StartCoroutine(ProcessSwipe());
        }
    }

    IEnumerator ProcessSwipe()
    {
        while (true)
        {
            int index = GetActualPostionIndex();
            if (Input.GetMouseButton(0))
            {
                scrollPos = scrollbar.value;
            }
            else
            {
                // Обрабатываем перемещение.
                Debug.Log(scrollbar.value);

                if (Mathf.Abs(scrollbar.value - positions[index]) > delta)
                {
                    scrollbar.value = Mathf.Lerp(scrollbar.value, positions[index], 3f * Time.deltaTime);
                }
                else
                {
                    scrollbar.value = positions[index];
                }
            }

            // Обрабатываем масштаб.
            for (int i = 0; i < positions.Length; i++)
            {
                float scale = i == index ? 1 : 0.8f;
                
                if (Mathf.Abs(transform.GetChild(i).localScale.x - scale) > delta)
                {
                    transform.GetChild(i).localScale = Vector3.Lerp(transform.GetChild(i).localScale, new Vector3(scale, scale, scale), 3f * Time.deltaTime);
                }
                else
                {
                    transform.GetChild(i).localScale = new Vector3(scale, scale, scale);
                }
            }

            if (CheckProcessSwipeFinish())
            {
                Debug.Log("Stop coroutine");
                coroutine = null;
                break;
            }

            yield return null;
        }
    }

    bool CheckProcessSwipeFinish()
    {
        bool checkPosition = false;
        bool checkScale = true;
        for (int i = 0; i < positions.Length; i++)
        {
            if (scrollbar.value == positions[i])
            {
                checkPosition = true;
            }

            if (transform.GetChild(i).localScale != new Vector3(0.8f, 0.8f, 0.8f)
                && transform.GetChild(i).localScale != Vector3.one)
            {
                checkScale = false;
            }
        }

        return checkPosition && checkScale;
    }
#endif

    int GetActualPostionIndex()
    {
        for (int i = 0; i < positions.Length; i++)
        {
            if (scrollPos > positions[i] - halfDistance && scrollPos < positions[i] + halfDistance)
            {
                return i;
            }
        }

        if (positions.Length == 0)
        {
            return -1;
        }
        else
        {
            return scrollPos < positions[0] ? 0 : positions.Length - 1;
        }
    }
}
