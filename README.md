# IAJ
Checkpoint 1 of Artificial Intelligence for Games
Grupo 6
Andre Fernandes - 78076
Sofia Aparicio - 81105
Rodrigo Lousada - 81115

The saved images shows the deep profiler result. This are the results after running the project 
with the Movement RVO on for 10 seconds and with 5 characters. After analizing the panel it 
was possible to realize that most of the CPU time was used by the RVOMovement, namly by 
this functions: get_Position, Vector3.op_Subtraction. 

When Deep profiler was turned on these movements had a frame around 170 ms. Regarding that 
these two functions were taking fare more time than the others, we improved them.

On the first, we saved the value of the position on a variable so it could be used without being 
called until the end of the loop iteration. By doing this we reduced the time frame to call this function
to around 16 ms.

On the second, we created a function that reduced the time of calculating the subtraction that reduced
the time frame to around 12ms.
