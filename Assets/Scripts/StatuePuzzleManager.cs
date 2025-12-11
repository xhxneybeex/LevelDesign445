using System.Collections;
using UnityEngine;

public class StatuePuzzleManager : MonoBehaviour
{
    [Header("Statue References")]
    public RotatableStatue horseStatue;
    public RotatableStatue lionStatue;
    public Transform humanStatue;
    
    [Header("Key Spawn")]
    public GameObject humanKeyPrefab;
    public Transform humanKeySpawnPoint;
    
    [Header("Scene Progression")]
    public GameObject haveHumanKeyObject;
    
    [Header("Check Settings")]
    public float checkInterval = 0.5f;
    
    [Header("Audio (Optional)")]
    public AudioClip puzzleSolvedSound;
    public AudioClip keySpawnSound;
    
    private bool puzzleSolved = false;
    private bool hasHorseKey = false;
    private bool hasLionKey = false;
    private AudioSource audioSource;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
        if (haveHumanKeyObject != null)
        {
            haveHumanKeyObject.SetActive(false);
        }
        
        Debug.Log($"[Puzzle Manager] Started. Horse Key: {hasHorseKey} | Lion Key: {hasLionKey}");
        Debug.Log($"[Puzzle Manager] Required angles - Horse: {(horseStatue != null ? horseStatue.correctRotationAngle.ToString("F1") : "N/A")}° | Lion: {(lionStatue != null ? lionStatue.correctRotationAngle.ToString("F1") : "N/A")}°");
        
        StartCoroutine(CheckPuzzleRoutine());
    }
    
    public void OnKeyCollected(ItemType keyType)
    {
        if (keyType == ItemType.HorseKey)
        {
            hasHorseKey = true;
            if (horseStatue != null)
            {
                horseStatue.EnableRotation(true);
                Debug.Log("Horse statue can now be rotated!");
            }
        }
        else if (keyType == ItemType.LionKey)
        {
            hasLionKey = true;
            if (lionStatue != null)
            {
                lionStatue.EnableRotation(true);
                Debug.Log("Lion statue can now be rotated!");
            }
        }
    }
    
    IEnumerator CheckPuzzleRoutine()
    {
        while (!puzzleSolved)
        {
            yield return new WaitForSeconds(checkInterval);
            
            if (hasHorseKey && hasLionKey)
            {
                CheckPuzzleSolution();
            }
        }
    }
    
    void CheckPuzzleSolution()
    {
        if (puzzleSolved) 
            return;
        
        bool horseCorrect = horseStatue != null && horseStatue.IsCorrectlyRotated();
        bool lionCorrect = lionStatue != null && lionStatue.IsCorrectlyRotated();
        
        Debug.Log($"[Puzzle Check] Horse: {(horseCorrect ? "CORRECT" : "incorrect")} | Lion: {(lionCorrect ? "CORRECT" : "incorrect")}");
        
        if (horseCorrect && lionCorrect)
        {
            PuzzleSolved();
        }
    }
    
    void PuzzleSolved()
    {
        puzzleSolved = true;
        Debug.Log("Puzzle solved! The human statue reveals the key!");
        
        if (audioSource != null && puzzleSolvedSound != null)
        {
            audioSource.PlayOneShot(puzzleSolvedSound);
        }
        
        StartCoroutine(SpawnHumanKeySequence());
    }
    
    IEnumerator SpawnHumanKeySequence()
    {
        yield return new WaitForSeconds(1f);
        
        if (humanKeyPrefab != null && humanKeySpawnPoint != null)
        {
            Vector3 spawnPos = humanKeySpawnPoint.position;
            Quaternion spawnRot = humanKeySpawnPoint.rotation;
            
            Debug.Log($"[Key Spawn] Spawning at position: {spawnPos}, rotation: {spawnRot.eulerAngles}");
            
            GameObject keyInstance = Instantiate(humanKeyPrefab, spawnPos, spawnRot);
            keyInstance.name = "HumanKey_Spawned";
            
            Debug.Log($"[Key Spawn] Key instantiated: {keyInstance.name}, Active: {keyInstance.activeSelf}");
            
            if (audioSource != null && keySpawnSound != null)
            {
                audioSource.PlayOneShot(keySpawnSound);
            }
            
            Debug.Log("Human key spawned!");
        }
        else
        {
            Debug.LogWarning($"[Key Spawn] Missing references! Prefab: {(humanKeyPrefab != null ? "OK" : "NULL")}, SpawnPoint: {(humanKeySpawnPoint != null ? "OK" : "NULL")}");
        }
    }
    
    public void OnHumanKeyCollected()
    {
        if (haveHumanKeyObject != null)
        {
            haveHumanKeyObject.SetActive(true);
            Debug.Log("HaveHumanKey object activated! You can now progress to the next scene.");
        }
    }
}
