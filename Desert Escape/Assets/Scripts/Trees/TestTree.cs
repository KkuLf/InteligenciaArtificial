using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTree : MonoBehaviour
{
    public int life;
    public int bullets;
    public bool los;
    ITreeNode _root;
    private void Awake()
    {
        InitializeTree();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _root.Execute();
        }
    }
    void InitializeTree()
    {
        //Actions
        ITreeNode dead = new ActionNode(() => print("Dead"));
        ITreeNode patrol = new ActionNode(() => print("Patrol"));
        ITreeNode reload = new ActionNode(() => print("Reload"));
        ITreeNode shoot = new ActionNode(() => print("Shoot"));

        //Questions
        ITreeNode qShoot = new QuestionNode(QuestionBullet, shoot, reload);
        ITreeNode qPratrol = new QuestionNode(QuestionBullet, patrol, reload);
        ITreeNode qLoS = new QuestionNode(QuestionLoS, qShoot, qPratrol);
        ITreeNode qHasLife = new QuestionNode(() => life > 0, qLoS, dead);

        _root = qHasLife;
    }

    public void ChangeTree(ITreeNode newTree)
    {
        _root = newTree;
    }

    public bool QuestionBullet()
    {
        return bullets > 0;
    }
    public bool QuestionLoS()
    {
        return los;
    }
}
