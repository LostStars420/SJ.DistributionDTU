/**
*             Copyright (C) SOJO Electric CO., Ltd. 2017-2018. All right reserved.
* @file:      rt_thread.h
* @brief:     作为中间层使用，代替系统函数，便于移植
* @version:   V0.1.0
* @author:    Zhang Yufei
* @date:      2018-06-25
* @update:
*/

#ifndef _RT_THREAD_SELF_
#define _RT_THREAD_SELF_

#include <stdlib.h>

#define  rt_malloc malloc
#define  rt_free  free
#define  rt_memset memset

#endif