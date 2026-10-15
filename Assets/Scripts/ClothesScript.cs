using NaughtyAttributes;
using UnityEngine;

[DefaultExecutionOrder(200)]
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
        
        
        if (head != null) _headRenderer = head.GetComponent<SpriteRenderer>();
        if (face != null) _faceRenderer = face.GetComponent<SpriteRenderer>();
        if (hat != null) _hatRenderer = hat.GetComponent<SpriteRenderer>();
        if (body != null) _bodyRenderer = body.GetComponent<SpriteRenderer>();
        if (chest != null) _chestRenderer = chest.GetComponent<SpriteRenderer>();
        if (lShoe != null) _leftShoeRenderer = lShoe.GetComponent<SpriteRenderer>();
        if (rShoe != null) _rightShoeRenderer = rShoe.GetComponent<SpriteRenderer>();
    }

    void LateUpdate()
    {
        FacingDirection facing = FacingDirection.Left;
        
        

        if (_playerDirection != null)
        {
            if (_playerMovement != null && _playerMovement.IsDashing)
                return;

            Vector2 dir = _playerDirection.direction;

            if (_playerDirection.IsHorizontal)
            {
                facing = _playerDirection.facingRight ? FacingDirection.Right : FacingDirection.Left;
                _playerMovement.dashDirection = _playerDirection.facingRight
                    ? PlayerMovement.MovementDirection.Right
                    : PlayerMovement.MovementDirection.Left;
            }
            else
            {
                facing = _playerDirection.facingUp ? FacingDirection.Back : FacingDirection.Front;
                _playerMovement.dashDirection = _playerDirection.facingUp
                    ? PlayerMovement.MovementDirection.Up
                    : PlayerMovement.MovementDirection.Down;
            }
        }
        if (_enemyAI != null)
        {
            Vector2 dir = _enemyAI.direction;
            float absX = Mathf.Abs(dir.x);
            float absY = Mathf.Abs(dir.y);

            if (_enemyAI.enemyHorizontal && absY > absX * (1f + _enemyAI.switchMargin))
                _enemyAI.enemyHorizontal = false;
            else if (!_enemyAI.enemyHorizontal && absX > absY * (1f + _enemyAI.switchMargin))
                _enemyAI.enemyHorizontal = true;

            if (_enemyAI.enemyHorizontal)
                facing = dir.x > 0 ? FacingDirection.Right : FacingDirection.Left;
            else
                facing = dir.y > 0 ? FacingDirection.Back : FacingDirection.Front;
        }
        


        // Face
        if (faceClothes != null && _faceRenderer != null)
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
        if (headClothes != null && _headRenderer != null)
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
        if (hatClothes != null && _hatRenderer != null)
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
        if (torsoClothes != null && _bodyRenderer != null)
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
        if (chestArmour != null && _chestRenderer != null)
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
        if (shoes != null && _leftShoeRenderer != null && _rightShoeRenderer != null)
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
    
    [Button("Randomize Clothes")]
    void RandomizeClothes()
    {
        _headRenderer = head.GetComponent<SpriteRenderer>();
        _faceRenderer = face.GetComponent<SpriteRenderer>();
        _hatRenderer = hat.GetComponent<SpriteRenderer>();
        _bodyRenderer = body.GetComponent<SpriteRenderer>();
        _chestRenderer = chest.GetComponent<SpriteRenderer>();
        _leftShoeRenderer = lShoe.GetComponent<SpriteRenderer>();
        _rightShoeRenderer = rShoe.GetComponent<SpriteRenderer>();

        FaceClothes[] faceClothesArray = Resources.LoadAll<FaceClothes>("Clothes/Face Acessories");
        HatClothes[]  hatClothesArray = Resources.LoadAll<HatClothes>("Clothes/Head Acessories");
        TorsoClothes[] torsoClothesArray = Resources.LoadAll<TorsoClothes>("Clothes/Shirt");
        ChestArmour[] chestArmoursArray = Resources.LoadAll<ChestArmour>("Clothes/Chest Armour");
        Shoes[] shoesArray  = Resources.LoadAll<Shoes>("Clothes/Shoes");
        
        float chestArmourvar = Random.Range(0, chestArmoursArray.Length + 1);
        
        faceClothes = faceClothesArray[Random.Range(0, faceClothesArray.Length)];
        hatClothes = hatClothesArray[Random.Range(0, hatClothesArray.Length)];
        torsoClothes = torsoClothesArray[Random.Range(0, torsoClothesArray.Length)];
        chestArmour = chestArmourvar < chestArmoursArray.Length ? chestArmoursArray[(int)chestArmourvar] : null;
        shoes = shoesArray[Random.Range(0, shoesArray.Length)];
        
        if (hatClothes != null)
        {
            _hatRenderer.sprite = hatClothes.right;
        }
        else
        {
            _hatRenderer.sprite = null;
        }

        if (faceClothes != null)
        {
            _faceRenderer.sprite = faceClothes.right;
        }
        else
        {
            _faceRenderer.sprite = null;
        }

        if (headClothes != null)
        {
            _headRenderer.sprite = headClothes.right;
        }
        else
        {
            _headRenderer.sprite = null;
        }

        if (torsoClothes != null)
        {
            _bodyRenderer.sprite = torsoClothes.right;
        }
        else
        {
            _bodyRenderer.sprite = null;
        }

        if (chestArmour != null)
        {
            _chestRenderer.sprite = chestArmour.right;
        }
        else
        {
            _chestRenderer.sprite = null;
        }

        if (shoes != null)
        {
            _leftShoeRenderer.sprite = shoes.right;
            _rightShoeRenderer.sprite = shoes.right;
        }
        else
        {
            _leftShoeRenderer.sprite = null;
            _rightShoeRenderer.sprite = null;
        }
    }
    
    [Button]
    void WearClothes()
    {
        _headRenderer = head.GetComponent<SpriteRenderer>();
        _faceRenderer = face.GetComponent<SpriteRenderer>();
        _hatRenderer = hat.GetComponent<SpriteRenderer>();
        _bodyRenderer = body.GetComponent<SpriteRenderer>();
        _chestRenderer = chest.GetComponent<SpriteRenderer>();
        _leftShoeRenderer = lShoe.GetComponent<SpriteRenderer>();
        _rightShoeRenderer = rShoe.GetComponent<SpriteRenderer>();
        
        if (hatClothes != null)
        {
            _hatRenderer.sprite = hatClothes.right;
        }
        else
        {
            _hatRenderer.sprite = null;
        }

        if (faceClothes != null)
        {
            _faceRenderer.sprite = faceClothes.right;
        }
        else
        {
            _faceRenderer.sprite = null;
        }

        if (headClothes != null)
        {
            _headRenderer.sprite = headClothes.right;
        }
        else
        {
            _headRenderer.sprite = null;
        }

        if (torsoClothes != null)
        {
            _bodyRenderer.sprite = torsoClothes.right;
        }
        else
        {
            _bodyRenderer.sprite = null;
        }

        if (chestArmour != null)
        {
            _chestRenderer.sprite = chestArmour.right;
        }
        else
        {
            _chestRenderer.sprite = null;
        }

        if (shoes != null)
        {
            _leftShoeRenderer.sprite = shoes.right;
            _rightShoeRenderer.sprite = shoes.right;
        }
        else
        {
            _leftShoeRenderer.sprite = null;
            _rightShoeRenderer.sprite = null;
        }
    }
}