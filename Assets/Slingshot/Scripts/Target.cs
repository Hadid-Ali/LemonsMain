using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public GameObject score;

    [SerializeField]
    private int scoreToAward = 100;
    private TextMesh _textMesh;

    private void OnValidate()
    {
        if (this.score == null)
            return;

        if (this._textMesh == null)
            this._textMesh = this.score.GetComponent<TextMesh>();

        this._textMesh.text = this.scoreToAward.ToString();
        this.score.name = this.scoreToAward.ToString();
    }

    void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.tag == "Sphere")
        {
            StartCoroutine(HitPointCenter());
            ScoreManager.instance.AddPoint(int.Parse(transform.GetChild(0).name));
        }
    }

    IEnumerator HitPointCenter()
    {
        score.SetActive(true);
        yield return new WaitForSeconds(2);
        score.SetActive(false);
    }
}