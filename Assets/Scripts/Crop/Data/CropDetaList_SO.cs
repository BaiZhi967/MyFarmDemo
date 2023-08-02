using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CropDetaList_SO",menuName = "Crop/CropDetaList")]
public class CropDetaList_SO : ScriptableObject
{
    public List<CropDetails> CropDetailsList;
}
