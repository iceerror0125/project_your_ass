using UnityEngine;

public static class PlayerUtils 
{
    public static bool IsPlayer(Collider other)
    {
        return other.CompareTag("Player");
    }
}
