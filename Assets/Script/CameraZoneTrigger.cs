using UnityEngine;
using Cinemachine;

public class CameraZoneTrigger : MonoBehaviour
{
    [Header("Ação Deste Gatilho")]
    [Tooltip("Marque esta caixa se este for o gatilho que AFASTA a câmera (Zoom Out). Desmarque se for o gatilho que APROXIMA a câmera de volta (Zoom In).")]
    public bool isZoomOutTrigger = true;

    [Header("Configuração de Câmeras")]
    [Tooltip("Arraste a CamPuzzle (Visão Afastada) aqui")]
    public CinemachineVirtualCamera puzzleCamera;

    [Tooltip("Prioridade normal da câmera (inativa).")]
    public int inactivePriority = 9;

    [Tooltip("Prioridade ao entrar na área (sobrepõe a câmera de ação).")]
    public int activePriority = 11;

    // Trava para que o trigger só seja ativado uma única vez
    private bool jaFoiAtivado = false;

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Se já ativou ou se a câmera não está definida, aborta
        if (jaFoiAtivado || puzzleCamera == null) return;

        if (collision.gameObject.layer == 9 || collision.gameObject.CompareTag("Player"))
        {
            // Ativa o bloqueio para que este chão não modifique a câmera repetidamente
            jaFoiAtivado = true;

            if (isZoomOutTrigger)
            {
                // AFASTAR: Eleva a prioridade da CamPuzzle, engolindo a câmera normal
                puzzleCamera.Priority = activePriority;
            }
            else
            {
                // APROXIMAR: Retira a prioridade da CamPuzzle, devolvendo a visão à câmera normal (Follow 10)
                puzzleCamera.Priority = inactivePriority;
            }
        }
    }
}