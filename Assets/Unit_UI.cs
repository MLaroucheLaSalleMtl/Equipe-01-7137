using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unit_UI : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Slider HP;
    [SerializeField] Image img;
    [SerializeField] Sprite[] src;
    [SerializeField] Text number;

    unit[] cachedUnit;
    public void Reset()
    {
        cachedUnit = null;
        gameObject.SetActive(false);
    }
    public void General(unit[] x, int z )
    {
        if (x == null || x.Length < 0) return;
        cachedUnit = x;
        gameObject.SetActive(true);
        this.enabled = true;
        if(z > 100)
        {
            z -= 100;
            z += 2;
        }
        img.sprite = src[z];
        var count = x.Length;
        var s = 0f;
        foreach (var item in cachedUnit)
            if (item != null)
                s += item.Hp;
            else count--;
        if (count <= 0) { Reset(); return; }  
        s /= count;
        HP.value = s / x[0].GetMaxmimumHP;

        number.text = count.ToString() + "x";
      
        
    }
    private void LateUpdate()
    {
        if (cachedUnit != null && Time.frameCount % 5 == 0) General(cachedUnit, cachedUnit[0].ID);
    }

}
