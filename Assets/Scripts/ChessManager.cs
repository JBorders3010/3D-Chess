﻿using UnityEngine;
using System.Collections.Generic;

public class ChessManager : MonoBehaviour {
    public GameObject[,] Spaces;   //tracks the game board spaces
    public PlayerClass Player1, Player2;
    public bool P1Turn;    //tracks the turns
    public bool P1Checkmate, P2Checkmate;  //checks for win. like above bools, P1.. = true is bad for P1 & P2.. = true is bad for P2
    private int P1Wins = 0, P2Wins = 0;
    private float ApplicationX, ApplicationY;   //window dimensions of game
    private float zoom, deltaZoom;
    private Vector3 lastPosition = new Vector3(0, 0, 0);    //tracks mouse position for camera pan
    public GameObject previousSelection, selectedPiece;
    public Material previousColor;
    private const float CAMERA_RADIUS = 8.6023252670426267717294735350497f;
    public bool SpaceSelectable, pieceSelectable;
    private string winLabel;

    // Use this for initialization
    void Start () {
        Spaces = new GameObject[8,8];
        Player1 = new PlayerClass();
        Player2 = new PlayerClass();
        GenerateBoard();
        P1Turn = true;
        SetCamera();
        zoom = 60f;
        deltaZoom = 0;
        ApplicationX = Screen.width;
        ApplicationY = Screen.height;
        SpaceSelectable = false;
        P1Checkmate = false;
        P2Checkmate = false;
        previousSelection = null;
        pieceSelectable = true;
        winLabel = "";
    }
	
    void Update () {
        ManagePerspective();
    }

    void OnGUI() {
        GUI.skin.label.alignment = TextAnchor.MiddleCenter;
        GUI.Label(new Rect(ApplicationX - 300, 0, 300, 20), "Player1: " + P1Wins + "\tPlayer2: " + P2Wins);
        if (!P1Checkmate && !P2Checkmate)
        {
            if (GUI.Button(new Rect(ApplicationX - 200, 22, 120, 30), "Resign"))
            {
                if (P1Turn)
                    P1Checkmate = true;
                else
                    P2Checkmate = true;
            }
        }        if (GUI.Button(new Rect(ApplicationX - 60, ApplicationY - 30, 60, 30), "Quit"))
            Application.Quit();
        if (GUI.Button(new Rect(0, ApplicationY - 30, 100, 30), "Reset Camera"))
            SetCamera();
        if (P1Checkmate)
        {
            Clear();
            winLabel = "Player 2 wins!!!!";
            P2Wins++;
            P1Checkmate = false;
        }
        else if (P2Checkmate)
        {
            Clear();
            winLabel = "Player 1 wins!!!!";
            P1Wins++;
            P2Checkmate = false;
        }
        GUI.Label(new Rect(ApplicationX / 2 - 50, ApplicationY / 2 - 12.5f, 100, 25), winLabel);
        if (GUI.Button(new Rect(ApplicationX - 160, ApplicationY / 2 - 15, 160, 30), "New Game"))
            Start();
    }

    void ManagePerspective() {
        //move camera around
        if (Input.GetMouseButtonDown(1))
            lastPosition = Input.mousePosition;
        if (Input.GetMouseButton(1))
        {
            Vector3 delta = Input.mousePosition - lastPosition;
            if ((transform.position.x < -4.5f && delta.x > 0) || (transform.position.x > 4.5f && delta.x < 0))
                delta.x = 0;
            if ((transform.position.z < -7.5f && delta.y > 0) || (transform.position.z > 7.5f && delta.y < 0))
                delta.y = 0;
            GameObject.Find("CameraCenter").transform.Translate(new Vector3(-delta.x * 2f, 0, -delta.y * 2f) * Time.deltaTime);
            //transform.position = new Vector3(transform.position.x, cameraY, transform.position.z);
            lastPosition = Input.mousePosition;
        }
        //rotate view of game board
        if (Input.GetMouseButtonDown(2))
            lastPosition = Input.mousePosition;
        if (Input.GetMouseButton(2))
        {
            Vector3 delta = Input.mousePosition - lastPosition;
            Vector3 angle = new Vector3(Mathf.PI - (2f * Mathf.Acos(delta.x / (2f * CAMERA_RADIUS))), 
                Mathf.PI - (2f * Mathf.Acos(delta.y / (2f * CAMERA_RADIUS))), 0f) * -300f;
            if (transform.position.y < 1)
            {
                if (transform.position.z < GameObject.Find("CameraCenter").transform.position.z && angle.y < 0)
                {
                    angle.y = 0;
                }
                else if (transform.position.z > GameObject.Find("CameraCenter").transform.position.z && angle.y > 0)
                {
                    angle.y = 0;
                }
            }
            GameObject.Find("CameraCenter").transform.Rotate(new Vector3(angle.y, -angle.x, 0f) * Time.deltaTime);
            lastPosition = Input.mousePosition;
        }
        //zoom in/out
        deltaZoom = -1f * Input.GetAxis("Mouse ScrollWheel") * 20f;
        zoom += deltaZoom;
        Camera.main.fieldOfView = Mathf.Clamp(zoom, 25f, 80f);
        //track window size
        ApplicationX = Screen.width;
        ApplicationY = Screen.height;
    }

    void GenerateBoard () {
        //sets board
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (i%2 == j % 2)
                {
                    Spaces[i, j] = (GameObject)Instantiate(Resources.Load("BrownSpace"));
                }
                else
                {
                    Spaces[i, j] = (GameObject)Instantiate(Resources.Load("TanSpace"));
                }  
                Spaces[i, j].transform.position = new Vector3(
                    Spaces[i, j].transform.position.x + j, Spaces[i, j].transform.position.y, Spaces[i, j].transform.position.z + i
                    );
            }
        }
        //sets Players pieces
        GameObject newPiece;
        for (int i = 0; i < 2; i++) //Player1
        {
            for (int j = 0; j < 8; j++)
            {
                if (i == 0)
                {
                    switch (j)
                    {
                        case 0:
                            Player1.AddPiece((GameObject)Instantiate(Resources.Load("P1Rook")), "rook", new int[] { i, j });
                            break;
                        case 1:
                            Player1.AddPiece((GameObject)Instantiate(Resources.Load("P1Knight")), "knight", new int[] { i, j });
                            break;
                        case 2:
                            Player1.AddPiece((GameObject)Instantiate(Resources.Load("P1Bishop")), "bishop", new int[] { i, j });
                            break;
                        case 3:
                            Player1.AddPiece((GameObject)Instantiate(Resources.Load("P1Queen")), "queen", new int[] { i, j });
                            break;
                        case 4:
                            Player1.AddPiece((GameObject)Instantiate(Resources.Load("P1King")), "king", new int[] { i, j });
                            break;
                        case 5:
                            newPiece = (GameObject)Instantiate(Resources.Load("P1Bishop"));
                            newPiece.transform.position = new Vector3(
                                -newPiece.transform.position.x, newPiece.transform.position.y, newPiece.transform.position.z
                                );
                            Player1.AddPiece(newPiece, "bishop", new int[] { i, j });
                            break;
                        case 6:
                            newPiece = (GameObject)Instantiate(Resources.Load("P1Knight"));
                            newPiece.transform.position = new Vector3(
                                -newPiece.transform.position.x, newPiece.transform.position.y, newPiece.transform.position.z
                                );
                            Player1.AddPiece(newPiece, "knight", new int[] { i, j });
                            break;
                        case 7:
                            newPiece = (GameObject)Instantiate(Resources.Load("P1Rook"));
                            newPiece.transform.position = new Vector3(
                                -newPiece.transform.position.x, newPiece.transform.position.y, newPiece.transform.position.z
                                );
                            Player1.AddPiece(newPiece, "rook", new int[] { i, j });
                            break;
                    }
                }
                else
                {
                    newPiece = (GameObject)Instantiate(Resources.Load("P1Pawn"));
                    newPiece.transform.position = new Vector3(
                        newPiece.transform.position.x + j, newPiece.transform.position.y, newPiece.transform.position.z
                        );
                    Player1.AddPiece(newPiece, "pawn", new int[] { i, j });
                }
            }
        }

        for (int i = 6; i < 8; i++) //Player2
        {
            for (int j = 0; j < 8; j++)
            {
                if (i == 7)
                {
                    switch (j)
                    {
                        case 0:
                            Player2.AddPiece((GameObject)Instantiate(Resources.Load("P2Rook")), "rook", new int[] { i, j });
                            break;
                        case 1:
                            Player2.AddPiece((GameObject)Instantiate(Resources.Load("P2Knight")), "knight", new int[] { i, j });
                            break;
                        case 2:
                            Player2.AddPiece((GameObject)Instantiate(Resources.Load("P2Bishop")), "bishop", new int[] { i, j });
                            break;
                        case 3:
                            Player2.AddPiece((GameObject)Instantiate(Resources.Load("P2Queen")), "queen", new int[] { i, j });
                            break;
                        case 4:
                            Player2.AddPiece((GameObject)Instantiate(Resources.Load("P2King")), "king", new int[] { i, j });
                            break;
                        case 5:
                            newPiece = (GameObject)Instantiate(Resources.Load("P2Bishop"));
                            newPiece.transform.position = new Vector3(
                                -newPiece.transform.position.x, newPiece.transform.position.y, newPiece.transform.position.z
                                );
                            Player2.AddPiece(newPiece, "bishop", new int[] { i, j });
                            break;
                        case 6:
                            newPiece = (GameObject)Instantiate(Resources.Load("P2Knight"));
                            newPiece.transform.position = new Vector3(
                                -newPiece.transform.position.x, newPiece.transform.position.y, newPiece.transform.position.z
                                );
                            Player2.AddPiece(newPiece, "knight", new int[] { i, j });
                            break;
                        case 7:
                            newPiece = (GameObject)Instantiate(Resources.Load("P2Rook"));
                            newPiece.transform.position = new Vector3(
                                -newPiece.transform.position.x, newPiece.transform.position.y, newPiece.transform.position.z
                                );
                            Player2.AddPiece(newPiece, "rook", new int[] { i, j });
                            break;
                    }
                }
                else
                {
                    newPiece = (GameObject)Instantiate(Resources.Load("P2Pawn"));
                    newPiece.transform.position = new Vector3(
                        newPiece.transform.position.x + j, newPiece.transform.position.y, newPiece.transform.position.z
                        );
                    Player2.AddPiece(newPiece, "pawn", new int[] { i, j });
                }
            }
        }
    }

    public void SetCamera () {
        if (P1Turn)
        {
            GameObject.Find("CameraCenter").transform.position = new Vector3(0f, 0f, 0f);
            GameObject.Find("CameraCenter").transform.eulerAngles = new Vector3(0f, 0f, 0f);
        }
        else
        {
            GameObject.Find("CameraCenter").transform.position = new Vector3(0f, 0f, 0f);
            GameObject.Find("CameraCenter").transform.eulerAngles = new Vector3(0f, 180f, 0f);
        }
        Camera.main.fieldOfView = 60f;
    }

    public void Move(Rigidbody pieceRB, GameObject Piece, Vector3 Target)
    {
        pieceRB.isKinematic = false;
        pieceRB.AddForce(ForceCalculation(pieceRB, Target, Piece.transform.position), ForceMode.Acceleration);
    }

    public void CheckPieceTake(Vector3 Target) {
        List<int[]> opposingPieces = new List<int[]>();
        if (P1Turn)
            opposingPieces = Player2.GetPiecePositions();
        else
            opposingPieces = Player1.GetPiecePositions();
        int i = 0;
        foreach (int[] space in opposingPieces)
        {
            if (Spaces[space[0], space[1]].transform.position.x == Target.x && 
                Spaces[space[0], space[1]].transform.position.z == Target.z)
            {
                GameObject[] pieces;
                if (P1Turn)
                {
                    pieces = Player2.GetPieces().ToArray();
                    Player2.RemovePiece(i);
                }
                else
                {
                    pieces = Player1.GetPieces().ToArray();
                    Player1.RemovePiece(i);
                }
                Destroy(pieces[i]);
                break;
            }
            i++;
        }
    }

    Vector3 ForceCalculation(Rigidbody piece, Vector3 Target, Vector3 startPos)
    {
        Vector3 initVelocity = new Vector3(XVelocity(Target, startPos), YVelocity(), ZVelocity(Target, startPos));
        Vector3 force = piece.mass * initVelocity / Time.fixedDeltaTime;
        return force;
    }

    float XVelocity(Vector3 endPos, Vector3 startPos)
    {
        float distance = endPos.x - startPos.x;
        return distance / (160f * Time.deltaTime);
    }

    float YVelocity()
    {
        return Physics.gravity.magnitude * (80f * Time.deltaTime);
    }

    float ZVelocity(Vector3 endPos, Vector3 startPos)
    {
        float distance = endPos.z - startPos.z;
        return distance / (160f * Time.deltaTime);
    }

    void Clear()
    {
        if (Spaces != null)
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Destroy(Spaces[i, j]);
                }
            }
        if (Player1 != null)
            foreach (GameObject piece in Player1.GetPieces())
                Destroy(piece);
        if (Player2 != null)
            foreach (GameObject piece in Player2.GetPieces())
                Destroy(piece);
    }
}
