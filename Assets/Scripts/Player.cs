using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour {
    [Header("Player Info")]
    public PlayerInfo playerInfo = new PlayerInfo();

    private Coroutine skipTurnCoroutine;
    private Coroutine moveQueenCoroutine;
    private Coroutine moveQueenTimerCoroutine;
    private CameraSystem cameraSystem;
    private LobbyManager lobbyManager;
    private TurnManager turnManager;
    private WeaponManager weaponManager;
    private QueenAntScript queenAntScript;

    public bool ConfirmedQueenSpawn { get; private set; } = false;
    private bool canSpawnQueen = false;
    private bool queenInValidPos = true;

    public GameObject QueenAnt { get; private set; } = null;
    public List<GameObject> AntList { get; private set; } = new List<GameObject>();
    public List<BaseWeaponSO> CurrentWeapons { get; private set; } = new List<BaseWeaponSO>();

    private void Awake() {
        lobbyManager = FindFirstObjectByType<LobbyManager>();
        SceneManager.activeSceneChanged += ActiveSceneChanged;

        DontDestroyOnLoad(gameObject);
    }

    private void ActiveSceneChanged(Scene currentScene, Scene nextScene) {
        if (nextScene.name.Contains("Game")) {
            turnManager = FindFirstObjectByType<TurnManager>();
            weaponManager = FindFirstObjectByType<WeaponManager>();
            cameraSystem = FindFirstObjectByType<CameraSystem>();            
        }
    }

    public void AddNewAnt(GameObject newAnt) {
        AntList.Add(newAnt);
    }

    public void RemoveAnt(GameObject ant) {
        AntList.Remove(ant);
    }

    public void AddQueen(GameObject newQueen) {
        QueenAnt = newQueen;
        queenAntScript = QueenAnt.GetComponent<QueenAntScript>();
    }

    public void RemoveQueen() {
        QueenAnt = null;
    }

    //Function called when the player presses the change color button
    private void OnChangeColor() {
        if (lobbyManager != null && SceneManager.GetActiveScene().name.Contains("Menu")) {
            lobbyManager.ChangeColor(playerInfo.playerNum); //Only works in the menu
        }
    }

    //Function called when the player presses the skip turn button
    private void OnSkipTurn() {
        if (CheckActionIsValid() && weaponManager.WeaponMenuOpen == false) {
            if (skipTurnCoroutine == null) {
                skipTurnCoroutine = StartCoroutine(SkipTurnCoroutine(2.0f));
                cameraSystem.ZoomCameraIn(2.0f);
            }
            else if (skipTurnCoroutine != null) {
                if (cameraSystem.IsZoomingOut == false) {
                    cameraSystem.ZoomCameraOut(0.5f);
                }

                StopCoroutine(skipTurnCoroutine);
                skipTurnCoroutine = null;
            }
        }
    }

    //Function called when the player presses the fire weapon button
    private void OnFireWeapon() {
        if (CheckActionIsValid() && weaponManager.WeaponMenuOpen == false && weaponManager.WeaponSelected != null) {
            if (weaponManager.WeaponSelected.isMelee) {
                weaponManager.UseMeleeWeapon(weaponManager.WeaponSelected, turnManager.CurrentAntTurn.transform);
            }
            else if (weaponManager.WeaponSelected.isSpray) {
                weaponManager.UseSprayWeapon(weaponManager.WeaponSelected, turnManager.CurrentAntTurn.transform);
            }
            else {
                weaponManager.FireWeapon(weaponManager.WeaponSelected, turnManager.CurrentAntTurn.transform);
            }
        }
    }

    //Function called when the player presses the move button
    private void OnMove(InputValue value) {
        if (CheckActionIsValid() && weaponManager.WeaponMenuOpen == false && weaponManager.WeaponSelected == null) {
            if (turnManager.CurrentAntTurn != null) {
                turnManager.CurrentAntTurn.OnMove(value);
			} 
        }
    }

    //Function called when the player presses the jump button
    private void OnJump() {
        if (CheckActionIsValid() && weaponManager.WeaponMenuOpen == false && weaponManager.WeaponSelected == null) {
            if (turnManager.CurrentAntTurn != null) {
                turnManager.CurrentAntTurn.OnJump();
            }
        }
    }

    //Function called when the player presses the open weapon menu button
    private void OnWeaponMenu() {
        if (CheckActionIsValid()) {
            weaponManager.WeaponMenu();
        }
    }

    //Function called when player has weapon selected and is aiming
    private void OnAim(InputValue value) {
        if (CheckActionIsValid() && weaponManager.WeaponMenuOpen == false && weaponManager.WeaponSelected != null) {
            weaponManager.AimWeapon(value);
        }
    }

    //Function called when player has weapon selected and changes strength of shot
    private void OnAimStrength(InputValue value) {
        if (CheckActionIsValid() && weaponManager.WeaponMenuOpen == false && weaponManager.WeaponSelected != null) {
            weaponManager.AimStrength(value);
        }
    }

    //Function called when selecting where to spawn queen
    private void OnSpawnQueenAnt() {
        if (canSpawnQueen == true && queenInValidPos == true) {
            canSpawnQueen = false;
            ConfirmedQueenSpawn = true;
            queenAntScript.SetQueenToTeamColour(playerInfo.playerColor);

            if (moveQueenCoroutine != null) {
                StopCoroutine(moveQueenCoroutine);
                moveQueenCoroutine = null;
            }

            if (moveQueenTimerCoroutine != null) { 
                StopCoroutine(moveQueenTimerCoroutine);
                moveQueenTimerCoroutine = null;
            }
        }
    }

    private void OnMoveQueen(InputValue value) {
        if (canSpawnQueen == true) {
            if (moveQueenCoroutine == null) {
                moveQueenCoroutine = StartCoroutine(MoveQueenCoroutine(value.Get<float>()));
            }
            else if (moveQueenCoroutine != null) {
                StopCoroutine(moveQueenCoroutine);
                moveQueenCoroutine = null;
            }
        }
    }

    private bool CheckActionIsValid() {
        if (turnManager != null && weaponManager != null && SceneManager.GetActiveScene().name.Contains("Game") && turnManager.CurrentPlayerTurn == this) {
            return true;
        }
        else {
            return false;
        }
    }

    public void StopSkipTurn() {
        if (skipTurnCoroutine != null) {
            StopCoroutine(skipTurnCoroutine);
        }    
    }

    private IEnumerator SkipTurnCoroutine(float skipDuration) {
        float timeElapsed = 0.0f;

        while (timeElapsed < skipDuration) {
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        cameraSystem.ZoomCameraOut(0.5f);
        turnManager.EndTurn();
    }

    private IEnumerator MoveQueenCoroutine(float value) {
        while (true) {
            if (value < 0.0f) {
                value = -1;
            }
            else if (value > 0.0f) {
                value = 1;
            }

            float xPos = QueenAnt.transform.position.x + value * 2.5f * Time.deltaTime;
            xPos = Mathf.Clamp(xPos, turnManager.MapMinX, turnManager.MapMaxX);
            Vector3 targetPos = new Vector3(xPos, 30.0f, 0);

            if (Physics.Raycast(targetPos, Vector3.down, out RaycastHit ray, 35.0f, ~queenAntScript.GetQueenLayerMask())) {
                QueenAnt.transform.position = new Vector3(ray.point.x, ray.point.y + 0.5f, ray.point.z);
            }

            CheckQueenInValidPos();

            yield return null;
        }
    }

    private void CheckQueenInValidPos() {
        if (Physics.OverlapSphere(QueenAnt.transform.position, turnManager.MinDistanceBetweenQueens).Where(x => x.CompareTag("Player")).Count() > 1) {
            queenInValidPos = false;
            queenAntScript.SetQueenInvalidPos();
        }
        else {
            queenInValidPos = true;
            queenAntScript.SetQueenValidPos();
        }
    }

    private IEnumerator MoveQueenTimerCoroutine() {
        float timeRemaining = 20.0f;

        while (timeRemaining > 0) {
            timeRemaining -= Time.deltaTime;
            turnManager.SetTurnTimerText(timeRemaining);

            yield return null;
        }

        while (queenInValidPos == false) {
            QueenAnt.transform.position = turnManager.GetAntSpawnPoint(turnManager.MinDistanceBetweenQueens, true);
            CheckQueenInValidPos();
        }

        OnSpawnQueenAnt();
    }

    public Ant GetAnt(Ant currentAnt) {
        if (currentAnt != null) {
            currentAnt.moveVector = Vector3.zero;
        }

        List<Ant> possibleAnts = new List<Ant>();
        for (int i = 0; i < AntList.Count; i++) {
			Ant nextAnt = AntList[i].GetComponent<Ant>();
            if (nextAnt.hasHadTurn == false) {
                possibleAnts.Add(nextAnt);
            }
        }

        if (possibleAnts.Count > 0) {
            int nextAntIndex = Random.Range(0, possibleAnts.Count);
            return possibleAnts[nextAntIndex];
        } else {
            Ant testAnt = QueenAnt.GetComponent<Ant>();

			if (testAnt.hasHadTurn) {
				return null;
            } else {
                return testAnt;

			}
            
        }
    }

    public void ResetAnts() {
        for (int i = 0; i < AntList.Count; i++) {
            AntList[i].GetComponent<Ant>().hasHadTurn = false;
        }
		QueenAnt.GetComponent<Ant>().hasHadTurn = false;
	}


    public void AddNewWeapon(BaseWeaponSO newWeapon) {
        CurrentWeapons.Add(newWeapon);
    }


    public void RemoveWeapon(BaseWeaponSO weapon) {
        CurrentWeapons.Remove(weapon);

    }

    public void AllowPlayerToSpawnQueen() {
        canSpawnQueen = true;
        queenAntScript.SetQueenValidPos();
        moveQueenTimerCoroutine = StartCoroutine(MoveQueenTimerCoroutine());
    }

    //Temporary Func/Keybind of Left Shift
    private void OnQueenAttack() {
        if (CheckActionIsValid()) {
			queenAntScript.SpecialAttack();
        }
    }


    private void OnInteract() {
        if (CheckActionIsValid() && turnManager.CurrentAntTurn.canInteract) { 
            Debug.Log("ButtonPress");
            turnManager.CurrentAntTurn.interactable.Interaction();
        }
    }



    [Header("Debug Thing: Please ignore")]
    [SerializeField]
    EffectScript EffectScript;
    private void OnEffectTest() {
        for(int i = 0; i < AntList.Count; i++) {
            EffectScript.AddEffect(AntList[i].GetComponent<Ant>());
            EffectScript.ApplyEffect(AntList[i].GetComponent<Ant>());

        }
		EffectScript.AddEffect(queenAntScript);
		EffectScript.ApplyEffect(queenAntScript);
	}
}