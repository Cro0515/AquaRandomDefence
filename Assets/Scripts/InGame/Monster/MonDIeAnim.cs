using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonDIeAnim : MonoBehaviour
{
    Animator m_anim;

    private void Awake()
    {

        m_anim = this.GetComponent<Animator>();

    }
    // Start is called before the first frame update
    void Start()
    {

        m_anim.SetBool("isDie", true);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
