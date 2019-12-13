using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EventTrigger))]
public class Window : MonoBehaviour
{
	public bool _movable = true;
	private Canvas _Canvas;
    private EventTrigger _eventTrigger;
	
    void Start ()
	{
        _eventTrigger = GetComponent<EventTrigger>();
		_eventTrigger.AddEventTrigger(OnDrag, EventTriggerType.Drag);
        
		Transform parent = gameObject.transform;
		
		while (parent != null)
		{
			_Canvas = parent.GetComponent<Canvas>();
			
			if (_Canvas != null)
			{
				break;
			}
			
			parent = parent.parent;
		}
    }

    void OnDrag(BaseEventData data)
	{
		if (!_movable)
		{
			return;
		}
    	
        PointerEventData ped = (PointerEventData) data;

		Vector3 delta = ped.delta;
		
		float scaleFactor = _Canvas.scaleFactor;
				
		float x = gameObject.transform.position.x;
		float y = gameObject.transform.position.y;
		float w = gameObject.GetComponent<RectTransform>().rect.width * scaleFactor;
		float h = gameObject.GetComponent<RectTransform>().rect.height * scaleFactor;
		
		float pw = gameObject.transform.parent.GetComponent<RectTransform>().rect.width * scaleFactor;
	    float ph = gameObject.transform.parent.GetComponent<RectTransform>().rect.height * scaleFactor;
		
	    float pivotAdjustmentX = gameObject.GetComponent<RectTransform>().pivot.x * w;
	    float pivotAdjustmentY = gameObject.GetComponent<RectTransform>().pivot.y * h;
		
	    float topdistance = (y+h-pivotAdjustmentY)+delta.y;
		float bottomdistance = (y-pivotAdjustmentY)+delta.y;
		float leftdistance = (x-pivotAdjustmentX)+delta.x;
	    float rightdistance = (x+w-pivotAdjustmentX)+delta.x;
					
		if (topdistance > ph && delta.y > 0)
		{
			return;
		}

		if (bottomdistance < 0 && delta.y < 0)
		{
			return;
		}
		
		if (leftdistance < 0 && delta.x < 0)
		{
			return;
		}
		
		if (rightdistance > pw && delta.x > 0)
		{
			return;
		}
		
	    this.gameObject.transform.Translate(delta);
	}
}
