using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Sandbox;

namespace dotasmsplayground
{
	public class TitanFallDuck : NetworkComponent
	{
		public TitanFallController Controller;

		public bool IsActive;
		public bool IsSliding;

		public TitanFallDuck( TitanFallController controller )
		{
			Controller = controller;
		}

		public virtual void PreTick() 
		{
			bool wants = Input.Down( InputButton.Duck );

			if ( wants != IsActive ) 
			{
				if ( wants ) TryDuck();
				else TryUnDuck();
			}

			if ( IsActive )
			{
				Controller.SetTag( "ducked" );
				Controller.EyePosLocal *= 0.5f;

				float combinedVelocity = Controller.Velocity.Abs().x + Controller.Velocity.Abs().y;
				if (!IsSliding && Controller.GroundEntity != null && combinedVelocity > Controller.SlideThreshold)
				{
					IsSliding = true;
					Controller.Velocity = Controller.Velocity * new Vector3(2f, 2f, 0f);
				} else if (IsSliding && combinedVelocity < Controller.SlideThreshold)
				{
					IsSliding = false;
				}

			}

		}

		protected virtual void TryDuck()
		{
			IsActive = true;
		}

		protected virtual void TryUnDuck()
		{
			var pm = Controller.TraceBBox( Controller.Position, Controller.Position, originalMins, originalMaxs );
			if ( pm.StartedSolid ) return;

			IsActive = false;
			IsSliding = false;
		}

		// Uck, saving off the bbox kind of sucks
		// and we should probably be changing the bbox size in PreTick
		Vector3 originalMins;
		Vector3 originalMaxs;

		public virtual void UpdateBBox( ref Vector3 mins, ref Vector3 maxs, float scale )
		{
			originalMins = mins;
			originalMaxs = maxs;

			if ( IsActive )
				maxs = maxs.WithZ( 36 * scale );
		}

		//
		// Coudl we do this in a generic callback too?
		//
		public virtual float GetWishSpeed()
		{
			if ( !IsActive ) return -1;
			return 64.0f;
		}
	}
}
