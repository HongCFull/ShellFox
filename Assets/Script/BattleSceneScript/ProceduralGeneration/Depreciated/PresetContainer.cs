using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PresetContainer : MonoBehaviour
{   
   [Tooltip("The ObjectSet e.g. trees")]
   public ObjectType[] objectType;

   [Tooltip("The corresponding percentage(%) of that objectType")]
   public float[] BoundPercentage;

   void Start() {
      ReportIfMissingPresetParameter();
   }

   void ReportIfMissingPresetParameter(){
      if(objectType.Length > BoundPercentage.Length )
         Debug.LogError("Please Set the BoundPercentage for the preset type");
      else if(objectType.Length < BoundPercentage.Length )
         Debug.LogError("Excessive BoundPercentage ");
   }

   public int TotalNumberOfObjectType(){
      return objectType.Length;
   }


}
