#pragma once
#include <stdint.h>
#include "distribution_def.h"
#include "log.pb.h"
/**
*�Զ�����Ϣ
*/
#define  FRAME_MESSAGE_MAX 200

typedef struct TagComselfMessage
{
    uint16_t sourceAddress; //��ַ
    uint16_t destAdrress; //Ŀ�ĵ�ַ
    uint8_t funcode; //���ܴ���
    uint16_t datalen; //���ݳ���
    uint8_t*  validDataPtr;//��Ч�����Ϊ200
    uint8_t completeFlag; //0-δ��� 
}ComselfMessage;



extern "C" _declspec(dllexport) uint8_t __stdcall ProtocolInit(uint16_t local_address);
extern "C" _declspec(dllexport) uint8_t __stdcall ProtocolAnylastDeal(uint8_t* inData, uint16_t inLen, uint16_t* usedLen, FrameRtu* rtu, uint16_t* sourceAddress);
extern "C" _declspec(dllexport) uint8_t __stdcall ProtocolAnylastMessageDeal(uint8_t* inData, uint16_t* inLen, uint16_t* usedLen, ComselfMessage* message);
extern "C" _declspec(dllexport) uint8_t __stdcall ParseLogMessage(uint8_t* data, uint16_t len, uint16_t* count);
extern "C" _declspec(dllexport) uint8_t __stdcall  GetLogMessage(ExceptionRecord* record, uint16_t* remindCount);
extern "C" _declspec(dllexport) uint8_t __stdcall  MakeEncodeStationMessage(uint32_t* pdata, uint8_t count, uint8_t** pEncode, uint16_t* usedLen);