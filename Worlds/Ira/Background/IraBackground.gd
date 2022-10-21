extends ParallaxBackground
var move_to = Vector2()
func _process(delta):
	#Rotation
	$Layer10/Sprite.rotation_degrees -= 2 /10
	$Layer9/Sprite.rotation_degrees += 1.9 /10
	$Layer8/Sprite.rotation_degrees -= 1.8 /10
	$Layer7/Sprite.rotation_degrees += 1.7 /10
	$Layer6/Sprite.rotation_degrees -= 1.6 /10
	$Layer5/Sprite.rotation_degrees += 1.5 /10
	$Layer4/Sprite.rotation_degrees -= 1.4 /10
	$Layer3/Sprite.rotation_degrees += 1.3 /10
	$Layer2/Sprite.rotation_degrees -= 1.2 /10
	$Layer1/Sprite.rotation_degrees += 1.1 /10
