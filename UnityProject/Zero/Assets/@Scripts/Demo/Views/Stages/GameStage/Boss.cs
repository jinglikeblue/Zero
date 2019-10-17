using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Zero;
using ILZero;

namespace ILDemo
{
    public class Boss : AView
    {
        float LIMIT_RIGHT = 1.12f;
        float LIMIT_LEFT = -1.12f;
        int MOVE_POWER = 50;
        float SPEED = 0.3f;


        public Rigidbody2D _body;
        public EdgeCollider2D _collider;

        void OnUpdate()
        {
            if(gameObject.transform.localPosition.x >= LIMIT_RIGHT)
            {
                _body.velocity = Vector2.left * SPEED;
            }
            else if(gameObject.transform.localPosition.x <= LIMIT_LEFT)
            {
                _body.velocity = Vector2.right * SPEED;
            }

            
        }       

        protected override void OnInit()
        {
            _body = GetComponent<Rigidbody2D>();
            _collider = GetComponent<EdgeCollider2D>();

            
        }

        protected override void OnEnable()
        {            
            _body.velocity = Vector2.right * SPEED;            
            ILBridge.Ins.onFixedUpdate += OnUpdate;
        }

        protected override void OnDisable()
        {            
            ILBridge.Ins.onFixedUpdate -= OnUpdate;
        }
    }
}
