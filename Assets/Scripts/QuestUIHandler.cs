//using UnityEngine;
//using TMPro;

//public class QuestUIHandler : MonoBehaviour
//{
//    private TextMeshProUGUI questText;

//    void Awake() => questText = GetComponent<TextMeshProUGUI>();

//    void OnEnable() => QuestManager.OnQuestUpdated += UpdateText;
//    void OnDisable() => QuestManager.OnQuestUpdated -= UpdateText;

//    private void UpdateText(string newText) => questText.text = newText;
//}