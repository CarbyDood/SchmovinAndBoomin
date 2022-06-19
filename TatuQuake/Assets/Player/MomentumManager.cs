using UnityEngine;
using TMPro;

public class MomentumManager : MonoBehaviour
{
    private float magnitude;
    private float magnitudeMax;

    [SerializeField] private float addCoeff = 0.002f;

    [SerializeField] private float subCoeff = 0.02f;

    [SerializeField] private TextMeshProUGUI momentumMeter;

    // Start is called before the first frame update
    private void Start()
    {
        magnitude = 0f;
        magnitudeMax = 100f;
    }

    private void Update() {
        //update momentum HUD element
        momentumMeter.text = ("Momentum: "+Mathf.Round(magnitude)+"%").ToString();
    }

    public void AddMomentum(float amount)
    {
        if(magnitude + amount > magnitudeMax)
            magnitude = magnitudeMax;
        else if(magnitude < magnitudeMax)
            magnitude += (amount * addCoeff);
    }

    public void SubMomentum(float amount)
    {
        if(magnitude - amount < 0)
            magnitude = 0;
        else if(magnitude > 0)
            magnitude -= (amount * subCoeff);
    }

    public float GetMomentum()
    {
        return magnitude;
    }

    public void SetMomentum(float amount)
    {
        magnitude = amount;
    }
}
