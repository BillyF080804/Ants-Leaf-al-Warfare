using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class QueenBaseAntScript : Ant {

    [Header("Queen Info")]
    [SerializeField] private LayerMask queenLayerMask;
    private int attackLevel = 1;
    private bool usedAttack = false;

    [Header("Queen Ant Settings")]
    [SerializeField] private QueenAttack AttackScript;
    [SerializeField] private List<GameObject> queenAntMeshes = new List<GameObject>();

    private Color queenColor;

    private void Awake() {
        foreach (GameObject obj in queenAntMeshes) {
            obj.SetActive(false);
        }
    }

    public void InitialiseQueenAttack() {
        QueenAttack tempAttack = Instantiate(antInfo.queenAttack);
        if (tempAttack.gameObject.GetComponent<BeeAttack>() != null) {
            AttackScript = this.AddComponent<BeeAttack>();
        }
        else if (tempAttack.gameObject.GetComponent<BulletAttack>() != null) {
            AttackScript = this.AddComponent<BulletAttack>();
        }
        else if (tempAttack.gameObject.GetComponent<DraculaAttack>() != null) {
            AttackScript = this.AddComponent<DraculaAttack>();
        }
        else if (tempAttack.gameObject.GetComponent<IceAttack>() != null) {
            AttackScript = this.AddComponent<IceAttack>();
        }
        else if (tempAttack.gameObject.GetComponent<PharaohAttack>() != null) {
            AttackScript = this.AddComponent<PharaohAttack>();
        }
        else if (tempAttack.gameObject.GetComponent<WeaverAttack>() != null) {
            AttackScript = this.AddComponent<WeaverAttack>();
        }

        AttackScript.InitialiseValues(tempAttack.gameObject);
        Destroy(tempAttack.gameObject);
    }

    public void CheckAttackTurn() {
        if (!usedAttack) {
            if (attackLevel < 3) {
                attackLevel++;
            }
        }

        usedAttack = false;
    }

    public void SpecialAttack() {
        if (attackLevel != 0) {
            if (antInfo.queenType == AntSO.QueenType.Dracula) {
                AttackScript.ActivateAttack(attackLevel, this);
            }
            else if (antInfo.queenType == AntSO.QueenType.Pharaoh) {
                AttackScript.ActivateAttack(attackLevel, this, transform.position, turnManager);
            }
            else {
                AttackScript.ActivateAttack(attackLevel, this, transform.position);
            }

            attackLevel = 0;
            usedAttack = true;
            turnManager.HideMainUI();
            StartCoroutine(turnManager.EndTurnCoroutine());
        }
    }

    public void SetAntColor(Color newColor) {
        queenColor = newColor;
    }

    public LayerMask GetQueenLayerMask() {
        return queenLayerMask;
    }

    public void ChangeQueenMesh(string queenType) {
        switch (queenType) {
            case "Bee":
                queenAntMeshes[1].SetActive(true);
                animator = queenAntMeshes[1].GetComponent<Animator>();
                ChangeQueenColor(queenAntMeshes[1].transform);
                break;
            case "Bullet":
                queenAntMeshes[2].SetActive(true);
                animator = queenAntMeshes[2].GetComponent<Animator>();
                ChangeQueenColor(queenAntMeshes[2].transform);
                break;
            case "Dracula":
                queenAntMeshes[3].SetActive(true);
                animator = queenAntMeshes[3].GetComponentInChildren<Animator>();
                ChangeQueenColor(queenAntMeshes[3].transform);
                break;
            case "Ice":
                queenAntMeshes[4].SetActive(true);
                animator = queenAntMeshes[4].GetComponentInChildren<Animator>();
                ChangeQueenColor(queenAntMeshes[4].transform);
                break;
            case "Pharaoh":
                queenAntMeshes[5].SetActive(true);
                animator = queenAntMeshes[5].GetComponentInChildren<Animator>();
                ChangeQueenColor(queenAntMeshes[5].transform);
                break;
            case "Weaver":
                queenAntMeshes[6].SetActive(true);
                animator = queenAntMeshes[6].GetComponentInChildren<Animator>();
                ChangeQueenColor(queenAntMeshes[6].transform);
                break;
            default:
                Debug.LogError("Invalid Queen Ant - " + queenType);
                queenAntMeshes[0].SetActive(true);
                animator = queenAntMeshes[0].GetComponent<Animator>();
                ChangeQueenColor(queenAntMeshes[0].transform);
                break;
        }
    }

    private void ChangeQueenColor(Transform transform) {
        Transform crown = transform.Find("QueenCrown");
        Transform leaf = transform.Find("QueenLeaf");

        if (crown == null) {
            crown = transform.GetChild(0).Find("QueenCrown");
        }

        if (leaf == null) {
            leaf = transform.GetChild(0).Find("QueenLeaf");
        }

        crown.GetComponent<SkinnedMeshRenderer>().material.SetColor("_MainColours", queenColor);
        leaf.GetComponent<SkinnedMeshRenderer>().material.SetColor("_MainColours", queenColor);
    }
}