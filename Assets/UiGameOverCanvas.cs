using UnityEngine;

public class UiGameOverCanvas : MonoBehaviour
{
   private Canvas _gameOverCanvas;

   private void Start()
   {
        _gameOverCanvas = GetComponent<Canvas>();
   }

   private void OnEnable()
   {
     PlayerController.GameOverEvent += GameOver;
   }

   private void OnDisable()
   {
    PlayerController.GameOverEvent -= GameOver;
   }

   private void GameOver()
   {
      _gameOverCanvas.enabled = true;
   }
}
