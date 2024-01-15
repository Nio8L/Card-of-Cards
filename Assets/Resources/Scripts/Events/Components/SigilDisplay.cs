using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SigilDisplay : MonoBehaviour
{
    public Sigil sigil;
    public Image image;
    public TextMeshProUGUI sigilName;
    public TextMeshProUGUI description;

    public void SetSigilDisplay(Sigil sigil){
        SetSigil(sigil);
        SetSigilUI();
    }

    public void SetSigilUI(){
        image.sprite = sigil.image;
        sigilName.text = sigil.sigilName;
        description.text = sigil.description;
    }

    public void SetSigil(Sigil sigil){
        this.sigil = sigil;
    }
}
