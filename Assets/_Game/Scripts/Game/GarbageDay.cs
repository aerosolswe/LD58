using GCG;
using UnityEngine;

public class GarbageDay : MonoBehaviour
{
    public AudioSource SFX;
    public Truck Truck;

    public float triggerDistance = 7.5f;

    private void Update()
    {
        bool heardBefore = UserDataManager.GetSavedValue("garbage_day_sfx", "0") == "1";

        if (!heardBefore)
        {
            float dst = Vector2.Distance(transform.position, Truck.transform.position);

            if (dst < triggerDistance)
            {
                SFX.Play();
                heardBefore = true;
                UserDataManager.SetSavedValue("garbage_day_sfx", "1");
            }
        }
    }
}
