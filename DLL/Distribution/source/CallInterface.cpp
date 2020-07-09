/**
*             Copyright (C) SOJO Electric CO., Ltd. 2017-2018. All right reserved.
* @file:      CallInterface.c
* @brief:     嵌入式与其他调用的（C#）接口中间层，方便C#调用减少不必要的转换。尤其是指针方面的转换
* @version:   V0.1.0
* @author:    Zhang Yufei
* @date:      2018-06-25
* @update:    
*/
#include "stdafx.h"

#include "CallInterface.h"

#include "distribution_def.h"
#include "distribution.h"
#include "RtuFrame.h"
#include "log.h"
#include "station.h"

static ProtocolAnylast LocalAnylast;
static NodeFifo* LocalFifo;


/**
* @brief : 协议解析初始化
* @param : uint16_t local_address 本地地址
* @return: uint8_t 0-成功
* @update: [2018-06-25] [创建]
*/
uint8_t __stdcall ProtocolInit(uint16_t local_address)
{
    uint8_t result = BuildNodeFifo(local_address, 1024, &LocalFifo);
    if (result)
    {
        return 0;
    }
    ProtocolAnylastInit(&LocalAnylast, local_address);
    LocalAnylast.fifohanlde = &(LocalFifo->reciveHandle);
    LocalAnylast.sendFifohanlde = &(LocalFifo->sendHandle);

    return 0;
}
/**
* @brief : 协议解析,处理剩余过多的问题
* @param : uint8_t* inData
* @param : uint16_t inLen
* @param : uint16_t* usedLen
* @param : uint8_t *outData
* @param : uint16_t *outLen
* @return: uint8_t 0xff 收到完整的一帧数据
* @update: [2018-06-25] [创建]
*/
uint8_t __stdcall ProtocolAnylastDeal(uint8_t* inData, uint16_t inLen, uint16_t* usedLen, FrameRtu* rtu, uint16_t* sourceAddress)
{
    uint8_t result;  
    FifoHandle* handle = LocalAnylast.fifohanlde;
    uint16_t i = 0;
    uint16_t resideLen = 0;
    uint16_t lastResideLen = 0;
    for (i = 0; i < inLen; i++)
    {
        result = handle->Enqueue(handle, inData[i]);

        if (!result)
        {
            break;
        }       
    }    
    *usedLen = i;
    memset(rtu, 0, sizeof(FrameRtu));
    do
    {
        LocalAnylast.recvRtu.completeFlag = 0;
        resideLen = LocalAnylast.ProtocolAnylastDeal(&LocalAnylast);
        if (LocalAnylast.recvRtu.completeFlag == TRUE)
        {     
            memcpy(rtu, &LocalAnylast.recvRtu, sizeof(FrameRtu));
           
            return 0;
        }

        //若相等，则退出
        if (lastResideLen == resideLen)
        {
            break;
        }
        lastResideLen = resideLen;
    } while (resideLen > 0);
  
    return 0;
}

/**
* @brief : 协议解析
* @param : uint8_t* inData
* @param : uint16_t* inLen
* @param : uint16_t* usedLen
* @param : ComselfMessage* message 通讯信息
* @return: 0--正常处理， 0x10-长度错误
* @update: [2018-06-25] [创建]
*/
uint8_t __stdcall ProtocolAnylastMessageDeal(uint8_t* inData, uint16_t* inLen, uint16_t* usedLen, ComselfMessage* message)
{
    uint8_t result;
    FifoHandle* handle = LocalAnylast.fifohanlde;
    uint16_t i = 0;
    uint16_t resideLen = 0;
    uint16_t lastResideLen = 0;
    for (i = 0; i < *inLen; i++)
    {
        result = handle->Enqueue(handle, inData[i]);

        if (!result)
        {
            break;
        }
    }
   

    do
    {
        resideLen = LocalAnylast.ProtocolAnylastDeal(&LocalAnylast);
        if (LocalAnylast.recvRtu.completeFlag == TRUE)
        {
            uint16_t dataL = LocalAnylast.recvRtu.pData[FRAME_SOURCE_INDEX];
            uint16_t dataH = LocalAnylast.recvRtu.pData[FRAME_SOURCE_INDEX + 1];
            uint16_t adress = (dataH << 8 | dataL);
            message->sourceAddress = adress;
            message->destAdrress = LocalAnylast.recvRtu.address;
            message->completeFlag = TRUE;
            message->funcode = LocalAnylast.recvRtu.funcode;
            message->datalen = LocalAnylast.recvRtu.datalen;
            if (LocalAnylast.recvRtu.datalen > FRAME_MESSAGE_MAX)
            {
                return 0x10;
            }
            message->validDataPtr = LocalAnylast.recvRtu.pData + FRAME_VALID_INDEX;
            return 0;
        }
        //若相等，则退出
        if (lastResideLen == resideLen)
        {
            break;
        }
        lastResideLen = resideLen;
    } while (resideLen > 0);

    return 0x01;
}
/**
* @brief : 协议解析
* @param : uint8_t* inData
* @param : uint16_t* inLen
* @param : uint16_t* usedLen
* @param : ComselfMessage* message 通讯信息
* @return: 0--正常处理， 0x10-长度错误
* @update: [2018-07-04] [创建]
*/
LogRecord Log;
int CurrentIndex = 0;
uint8_t __stdcall  ParseLogMessage(uint8_t* data, uint16_t len, uint16_t* count)
{       
    ErrorCode error = PacketDecodeLogMessage(&Log, data, len);
    if (error == ERROR_OK_NULL)
    {
        *count = Log.exception_count;
        CurrentIndex = 0;
        return 0;
    }
}
/**
* @brief : 协议解析
* @param : uint8_t* inData
* @param : uint16_t* inLen
* @param : uint16_t* usedLen
* @param : ComselfMessage* message 通讯信息
* @return: 0--正常处理, 非0未收到
* @update: [2018-07-04] [创建]
*/
uint8_t __stdcall  GetLogMessage(ExceptionRecord* record, uint16_t* remindCount)
{
    if (CurrentIndex < Log.exception_count)
    {
        //record = Log.exception + CurrentIndex;
        memcpy(record, Log.exception + CurrentIndex, sizeof(ExceptionRecord));
        CurrentIndex++;
    }
    else
    {
        return 0xFF;
    }
   
    *remindCount = Log.exception_count - CurrentIndex;
    
    return 0;
}



/**
* @brief : 生成状态信息数据
* @param :
* @return: 0--正常处理, 非0未收到
* @update: [2018-07-05] [创建]
*/
uint8_t __stdcall  MakeEncodeStationMessage(uint32_t* pdata, uint8_t count, uint8_t** pEncode, uint16_t* usedLen)
{
    PointUint8 packet;
    ErrorCode error = PacketEncodeStationMessage(pdata, count, &packet, 0, 0);
    if (error == ERROR_OK_NULL)
    {
        *pEncode = packet.pData;
        *usedLen = packet.len;
        return 0;
    }
    return 0x10;

}