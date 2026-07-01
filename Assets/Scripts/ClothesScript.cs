using UnityEngine;

public class ClothesScript : MonoBehaviour
{
    private PlayerDirection _playerDirection;
    private PlayerMovement _playerMovement;

    private EnemyAI _enemyAI;

    [Header("Head Accessories")]
    public GameObject head;
    public GameObject face;
    public GameObject hat;

    [Space(4)]
    [Header("Accessories")]
    public HeadClothes headClothes;
    public FaceClothes faceClothes;
    public HatClothes hatClothes;

    [Header("Body Accessories")]
    public GameObject body;
    public GameObject chest;

    [Space(4)]
    [Header("Accessories")]
    public ChestArmour chestArmour;
    public TorsoClothes torsoClothes;

    [Header("Shoes Accessories")]
    public GameObject lShoe;
    public GameObject rShoe;

    [Space(4)]
    [Header("Accessories")]
    public Shoes shoes;

    private SpriteRenderer _headRenderer;
    private SpriteRenderer _faceRenderer;
    private SpriteRenderer _hatRenderer;
    private SpriteRenderer _bodyRenderer;
    private SpriteRenderer _chestRenderer;
    private SpriteRenderer _leftShoeRenderer;
    private SpriteRenderer _rightShoeRenderer;

    private enum FacingDirection
    {
        Left,
        Right,
        Front,
        Back
    }

    private void Start()
    {
        _playerDirection = GetComponent<PlayerDirection>();
        _playerMovement = GetComponent<PlayerMovement>();
        _enemyAI = GetComponent<EnemyAI>();
        
        
        _headRenderer = head.GetComponent<SpriteRenderer>();
        _faceRenderer = face.GetComponent<SpriteRenderer>();
        _hatRenderer = hat.GetComponent<SpriteRenderer>();
        _bodyRenderer = body.GetComponent<SpriteRenderer>();
        _chestRenderer = chest.GetComponent<SpriteRenderer>();
        _leftShoeRenderer = lShoe.GetComponent<SpriteRenderer>();
        _rightShoeRenderer = rShoe.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        FacingDirection facing = FacingDirection.Left;

        if (_playerDirection != null)
        {
            if (Mathf.Abs(_playerDirection.direction.x) >
                Mathf.Abs(_playerDirection.direction.y))
            {
                facing = _playerDirection.direction.x > 0
                    ? FacingDirection.Right
                    : FacingDirection.Left;
            }
            else
            {
                facing = _playerDirection.direction.y > 0
                    ? FacingDirection.Back
                    : FacingDirection.Front;
            }  
            
            if (_playerMovement.IsDashing)
                return;
        }
        if (_enemyAI != null)
        {
            if (Mathf.Abs(_enemyAI.direction.x) >
                Mathf.Abs(_enemyAI.direction.y))
            {
                facing = _enemyAI.direction.x > 0
                    ? FacingDirection.Right
                    : FacingDirection.Left;
            }
            else
            {
                facing = _enemyAI.direction.y > 0
                    ? FacingDirection.Back
                    : FacingDirection.Front;
            }
        }
        


        // Face
        if (faceClothes != null)
        {
            switch (facing)
            {
                case FacingDirection.Left:  _faceRenderer.sprite = faceClothes.left; break;
                case FacingDirection.Right: _faceRenderer.sprite = faceClothes.right; break;
                case FacingDirection.Front: _faceRenderer.sprite = faceClothes.front; break;
                case FacingDirection.Back:  _faceRenderer.sprite = faceClothes.back; break;
            }
        }

        // Head
        if (headClothes != null)
        {
            switch (facing)
            {
                case FacingDirection.Left:  _headRenderer.sprite = headClothes.left; break;
                case FacingDirection.Right: _headRenderer.sprite = headClothes.right; break;
                case FacingDirection.Front: _headRenderer.sprite = headClothes.front; break;
                case FacingDirection.Back:  _headRenderer.sprite = headClothes.back; break;
            }
        }

        // Hat
        if (hatClothes != null)
        {
            switch (facing)
            {
                case FacingDirection.Left:  _hatRenderer.sprite = hatClothes.left; break;
                case FacingDirection.Right: _hatRenderer.sprite = hatClothes.right; break;
                case FacingDirection.Front: _hatRenderer.sprite = hatClothes.front; break;
                case FacingDirection.Back:  _hatRenderer.sprite = hatClothes.back; break;
            }
        }

        // Body
        if (torsoClothes != null)
        {
            switch (facing)
            {
                case FacingDirection.Left:  _bodyRenderer.sprite = torsoClothes.left; break;
                case FacingDirection.Right: _bodyRenderer.sprite = torsoClothes.right; break;
                case FacingDirection.Front: _bodyRenderer.sprite = torsoClothes.front; break;
                case FacingDirection.Back:  _bodyRenderer.sprite = torsoClothes.back; break;
            }
        }

        // Chest
        if (chestArmour != null)
        {
            switch (facing)
            {
                case FacingDirection.Left:  _chestRenderer.sprite = chestArmour.left; break;
                case FacingDirection.Right: _chestRenderer.sprite = chestArmour.right; break;
                case FacingDirection.Front: _chestRenderer.sprite = chestArmour.front; break;
                case FacingDirection.Back:  _chestRenderer.sprite = chestArmour.back; break;
            }
        }

        // Shoes
        if (shoes != null)
        {
            
            switch (facing)
            {
                case FacingDirection.Left:
                    _leftShoeRenderer.sprite = shoes.left;
                    _rightShoeRenderer.sprite = shoes.left;
                    break;

                case FacingDirection.Right:
                    _leftShoeRenderer.sprite = shoes.right;
                    _rightShoeRenderer.sprite = shoes.right;
                    break;

                case FacingDirection.Front:
                    _leftShoeRenderer.sprite = shoes.front;
                    _rightShoeRenderer.sprite = shoes.front;
                    break;

                case FacingDirection.Back:
                    _leftShoeRenderer.sprite = shoes.back;
                    _rightShoeRenderer.sprite = shoes.back;
                    break;
            }
        }
    }
}