extends Area2D




func _on_TeleportArea_body_entered(body):
	if body.global_position.x > 0:
		body.global_position = $Teleport_Right.global_position
	elif body.global_position.x < 0:
		body.global_position = $Teleport_Left.global_position
