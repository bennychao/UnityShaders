# CinemaChine

## 原理

1. 在一个原始Camera上追加 CinemaChineBrain 脚本
![](./Docs/img/CameraChine_2020-04-08-22-24-26.png)



2. 定义虚拟Camera

![](./Docs/img/CameraChine_2020-04-08-22-26-13.png)


3. Timeline

定义多个虚拟Camera之后，追加Timeline的CinemaChine Slot，可以在多个机位之间进行切换。

![](./Docs/img/CameraChine_2020-04-08-22-27-14.png)



4. 状态机 Brain

其中的State是对应Animator中定义的
支持父子状态

![](./Docs/img/CameraChine_2020-04-08-22-42-31.png)

![](./Docs/img/CameraChine_2020-04-08-22-42-41.png)


![](./Docs/img/CameraChine_2020-04-08-22-42-51.png)