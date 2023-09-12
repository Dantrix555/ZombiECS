using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSingleton : MonoBehaviour
{
    #region Fields and Properties

    public static CameraSingleton Instance;

    [SerializeField]
    private float _startRadius;
    [SerializeField]
    private float _endRadius;
    [SerializeField]
    private float _startHeight;
    [SerializeField]
    private float _endHeight;
    [SerializeField]
    private float _speed;

    public float Speed => _speed;

    #endregion

    #region Public Methods

    public float RadiusAtScale(float scale) => Mathf.Lerp(_startRadius, _endRadius, 1 - scale);
    public float HeightAtScale(float scale) => Mathf.Lerp(_startHeight, _endHeight, 1 - scale);

    #endregion

    #region Unity Methods

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    #endregion
}
