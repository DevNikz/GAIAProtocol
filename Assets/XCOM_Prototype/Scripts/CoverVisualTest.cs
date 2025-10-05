using System.Collections;
using System.Collections.Generic;
using CodeMonkey.Utils;
using UnityEngine;

public class CoverVisualTest : MonoBehaviour {

    [SerializeField] private Sprite fullCoverSprite;
    [SerializeField] private Sprite halfCoverSprite;

    private SpriteRenderer[] spriteRendererArray;

    private void Awake() {
        spriteRendererArray = new SpriteRenderer[] {
            transform.Find("Sprite").GetComponent<SpriteRenderer>(),
            transform.Find("Sprite_U").GetComponent<SpriteRenderer>(),
            transform.Find("Sprite_D").GetComponent<SpriteRenderer>(),
            transform.Find("Sprite_L").GetComponent<SpriteRenderer>(),
            transform.Find("Sprite_R").GetComponent<SpriteRenderer>(),
        };
    }

    private void Update() {
        Vector3 worldPosition = UtilsClass.GetMouseWorldPosition();
        Vector2Int gridPosition = LevelGrid.Instance.GetGridPosition(worldPosition);
        if (LevelGrid.Instance.IsValidGridPosition(gridPosition)) {
            Vector3 snappedWorldPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);

            transform.position = snappedWorldPosition;

            CoverType coverType = LevelGrid.Instance.GetCoverTypeAtPosition(snappedWorldPosition);
            switch (coverType) {
                default:
                case CoverType.None:
                    foreach (SpriteRenderer spriteRenderer in spriteRendererArray) {
                        spriteRenderer.enabled = false;
                    }
                    break;
                case CoverType.Half:
                    foreach (SpriteRenderer spriteRenderer in spriteRendererArray) {
                        spriteRenderer.enabled = true;
                        spriteRenderer.sprite = halfCoverSprite;
                    }
                    break;
                case CoverType.Full:
                    foreach (SpriteRenderer spriteRenderer in spriteRendererArray) {
                        spriteRenderer.enabled = true;
                        spriteRenderer.sprite = fullCoverSprite;
                    }
                    break;
            }
        }
    }

}