using UnityEngine;

public static class MelodyGenerator
{
    public static int[][] scales = new int[][] {
        new int[] { 0, 2, 4, 5, 7, 9, 11 },  // major
        new int[] { 0, 2, 3, 5, 7, 8, 10 }   // minor
    };
    
    private static int[] currentScale = scales[0];
    private static int currentScaleIndex = 0;
    
    private static int currentNoteIndex = 0;
    
    private static float lastNoteTime = 0f;
    public static float inactivityThreshold = 2f;
    
    private static int lastMove = 0;


    public static int GetNextNote()
    {
        if (Time.time - lastNoteTime > inactivityThreshold)
        {
            ChangeScale();
            currentNoteIndex = 0;
        }
        
        lastNoteTime = Time.time;
        
        int move = 0;
        float rand = UnityEngine.Random.value;

        if (lastMove > 0)
        {
            if (rand < 0.5f) move = 1;
            else if (rand < 0.8f) move = 0;
            else move = -1;
        }
        else if (lastMove < 0)
        {
            if (rand < 0.5f) move = -1;
            else if (rand < 0.8f) move = 0;
            else move = 1;
        }
        else
        {
            if (rand < 0.33f) move = -1;
            else if (rand < 0.66f) move = 0;
            else move = 1;
        }
        
        int newIndex = currentNoteIndex + move;
        newIndex = Mathf.Clamp(newIndex, 0, currentScale.Length - 1);
        
        lastMove = newIndex - currentNoteIndex;
        currentNoteIndex = newIndex;
        
        return currentScale[currentNoteIndex];
    }
    
    private static void ChangeScale()
    {
        int newScaleIndex = UnityEngine.Random.Range(0, scales.Length);
        if (newScaleIndex == currentScaleIndex)
        {
            newScaleIndex = (currentScaleIndex + 1) % scales.Length;
        }
        currentScaleIndex = newScaleIndex;
        currentScale = scales[currentScaleIndex];

      //  Debug.Log("Scale changed to index " + currentScaleIndex);
    }
}