#pragma once
#include <stdint.h>
#include "distribution_def.h"
#include "log.pb.h"
/**
*自定义消息
*/
#define  FRAME_MESSAGE_MAX 200

typedef struct TagComselfMessage
{
    uint16_t sourceAddress; //地址
    uint16_t destAdrress; //目的地址
    uint8_t funcode; //功能代码
    uint16_t datalen; //数据长度
    uint8_t*  validDataPtr;//有效数据最长为200
    uint8_t completeFlag; //0-未完成 
}ComselfMessage;



extern "C" _declspec(dllexport) uint8_t __stdcall ProtocolInit(uint16_t local_address);
extern "C" _declspec(dllexport) uint8_t __stdcall ProtocolAnylastDeal(uint8_t* inData, uint16_t inLen, uint16_t* usedLen, FrameRtu* rtu, uint16_t* sourceAddress);
extern "C" _declspec(dllexport) uint8_t __stdcall ProtocolAnylastMessageDeal(uint8_t* inData, uint16_t* inLen, uint16_t* usedLen, ComselfMessage* message);
extern "C" _declspec(dllexport) uint8_t __stdcall ParseLogMessage(uint8_t* data, uint16_t len, uint16_t* count);
extern "C" _declspec(dllexport) uint8_t __stdcall  GetLogMessage(ExceptionRecord* record, uint16_t* remindCount);
extern "C" _declspec(dllexport) uint8_t __stdcall  MakeEncodeStationMessage(uint32_t* pdata, uint8_t count, uint8_t** pEncode, uint16_t* usedLen);