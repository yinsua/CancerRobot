`2015-08-11`
#癌症机器人项目传输协议
##暂时使用UDP通信

## `上位机IP及端口`
## no1 192.168.1.101  8023
## no2 192.168.1.102  8023
## no3 192.168.1.103  8023

## `下位机IP及端口`
## Arduino 192.168.1.106  8023

1. 引用communicate空间
2. 创建Communicate类
3. 调用类方法

###API说明
* 创建类时三个参数分别是目标IP、目标端口、本地端口
* 发送Gcode：sendGcode(Gcode)  (注：参数Gcode是类似这样的`字符串`："G01 X-20 Y30 Z60")
* 设置寄存器：WriteReg(regNumber,status)  (注：regNumber是寄存器编号`数字`，status是要设置的状态0或者1的`数字`)
* 读寄存器：ReadReg(regNumber)  (注：regNumber是寄存器编号`数字`)
