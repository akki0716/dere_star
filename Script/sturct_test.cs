using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sturct_test : MonoBehaviour {

    public notes[] note_property = new notes[2];

    // Use this for initialization
    void Start () {
		notes new_note1 = new notes();
		notes new_note2 = new notes(10, 10,true);

        Debug.Log("new_note1.time " + new_note1.time + " new_note1.y " + new_note1.y);
        Debug.Log("new_note2.time " + new_note2.time + " new_note2.y " + new_note2.y + " new_note2.state " + new_note2.state);

        note_property[0] = new_note1;
        //note_property[1] = new_note2;
        Debug.Log(note_property[0]);
        //Debug.Log(note_property[1]);
        new_note2 = new notes(5,5,false);
        Debug.Log("new_note2.time " + new_note2.time + " new_note2.y " + new_note2.y + " new_note2.state " + new_note2.state);

    }
	
	// Update is called once per frame
	void Update () {
		
	}



	public struct notes
	{
        public int time, y;
        public bool state;

		public notes(int p1, int p2, bool st)
		{
			time = p1;
			y = p2;
            state = st;

        }
        /*
        public static implicit operator int(notes v)
        {
            throw new NotImplementedException();
        }
        */
    }
}
