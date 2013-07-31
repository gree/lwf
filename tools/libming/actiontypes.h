/*
    Ming, an SWF output library
    Copyright (C) 2006  netSweng, LLC - http://www.netsweng.com/

    This library is free software; you can redistribute it and/or
    modify it under the terms of the GNU Lesser General Public
    License as published by the Free Software Foundation; either
    version 2.1 of the License, or (at your option) any later version.

    This library is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
    Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public
    License along with this library; if not, write to the Free Software
    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
*/

#ifndef SWF_ACTIONTYPES_H_INCLUDED
#define SWF_ACTIONTYPES_H_INCLUDED

#include "ming.h"

typedef enum
{
  SWFACTION_END        = 0x00,

/* v3 actions */
  SWFACTION_NEXTFRAME     = 0x04,
  SWFACTION_PREVFRAME     = 0x05,
  SWFACTION_PLAY          = 0x06,
  SWFACTION_STOP          = 0x07,
  SWFACTION_TOGGLEQUALITY = 0x08,
  SWFACTION_STOPSOUNDS    = 0x09,
  SWFACTION_GOTOFRAME     = 0x81, /* >= 0x80 means record has args */
  SWFACTION_GETURL        = 0x83,
  SWFACTION_WAITFORFRAME  = 0x8A,
  SWFACTION_SETTARGET     = 0x8B,
  SWFACTION_GOTOLABEL     = 0x8C,

/* v4 actions */
  SWFACTION_ADD                     = 0x0A,
  SWFACTION_SUBTRACT                = 0x0B,
  SWFACTION_MULTIPLY                = 0x0C,
  SWFACTION_DIVIDE                  = 0x0D,
  SWFACTION_EQUAL                   = 0x0E,
  SWFACTION_LESSTHAN                = 0x0F,
  SWFACTION_LOGICALAND              = 0x10,
  SWFACTION_LOGICALOR               = 0x11,
  SWFACTION_LOGICALNOT              = 0x12,
  SWFACTION_STRINGEQ                = 0x13,
  SWFACTION_STRINGLENGTH            = 0x14,
  SWFACTION_SUBSTRING               = 0x15,
  SWFACTION_POP                     = 0x17,
  SWFACTION_INT                     = 0x18,
  SWFACTION_GETVARIABLE             = 0x1C,
  SWFACTION_SETVARIABLE             = 0x1D,
  SWFACTION_SETTARGET2              = 0x20,
  SWFACTION_STRINGCONCAT            = 0x21,
  SWFACTION_GETPROPERTY             = 0x22,
  SWFACTION_SETPROPERTY             = 0x23,
  SWFACTION_DUPLICATECLIP           = 0x24,
  SWFACTION_REMOVECLIP              = 0x25,
  SWFACTION_TRACE                   = 0x26,
  SWFACTION_STARTDRAG               = 0x27,
  SWFACTION_ENDDRAG                 = 0x28,
  SWFACTION_STRINGCOMPARE           = 0x29,
  SWFACTION_RANDOMNUMBER            = 0x30,
  SWFACTION_MBLENGTH                = 0x31,
  SWFACTION_ORD                     = 0x32,
  SWFACTION_CHR                     = 0x33,
  SWFACTION_GETTIME                 = 0x34,
  SWFACTION_MBSUBSTRING             = 0x35,
  SWFACTION_MBORD                   = 0x36,
  SWFACTION_MBCHR                   = 0x37,

  SWFACTION_WAITFORFRAME2           = 0x8D,
  SWFACTION_PUSH                    = 0x96,
  SWFACTION_JUMP                    = 0x99,
  SWFACTION_GETURL2                 = 0x9A,
  SWFACTION_IF                      = 0x9D,
  SWFACTION_CALLFRAME               = 0x9E,
  SWFACTION_GOTOFRAME2              = 0x9F,

/* v5 actions */
  SWFACTION_DELETE                  = 0x3A,
  SWFACTION_DELETE2                 = 0x3B,
  SWFACTION_DEFINELOCAL             = 0x3C,
  SWFACTION_CALLFUNCTION            = 0x3D,
  SWFACTION_RETURN                  = 0x3E,
  SWFACTION_MODULO                  = 0x3F,
  SWFACTION_NEWOBJECT               = 0x40,
  SWFACTION_NEWMETHOD               = 0x53,
  SWFACTION_DEFINELOCAL2            = 0x41,
  SWFACTION_INITARRAY               = 0x42,
  SWFACTION_INITOBJECT              = 0x43,
  SWFACTION_TYPEOF                  = 0x44,
  SWFACTION_TARGETPATH              = 0x45,
  SWFACTION_ENUMERATE               = 0x46,
  SWFACTION_ADD2                    = 0x47,
  SWFACTION_LESS2                   = 0x48,
  SWFACTION_EQUALS2                 = 0x49,
  SWFACTION_TONUMBER                = 0x4A,
  SWFACTION_TOSTRING                = 0x4B,
  SWFACTION_PUSHDUP                 = 0x4C,
  SWFACTION_STACKSWAP               = 0x4D,
  SWFACTION_GETMEMBER               = 0x4E,
  SWFACTION_SETMEMBER               = 0x4F,
  SWFACTION_INCREMENT               = 0x50,
  SWFACTION_DECREMENT               = 0x51,
  SWFACTION_CALLMETHOD              = 0x52,
  SWFACTION_BITWISEAND              = 0x60,
  SWFACTION_BITWISEOR               = 0x61,
  SWFACTION_BITWISEXOR              = 0x62,
  SWFACTION_SHIFTLEFT               = 0x63,
  SWFACTION_SHIFTRIGHT              = 0x64,
  SWFACTION_SHIFTRIGHT2             = 0x65,

  SWFACTION_STOREREGISTER           = 0x87,
  SWFACTION_CONSTANTPOOL            = 0x88,
  SWFACTION_WITH                    = 0x94,
  SWFACTION_DEFINEFUNCTION          = 0x9B,

/* v6 actions */
  SWFACTION_INSTANCEOF              = 0x54,
  SWFACTION_ENUMERATE2              = 0x55,
  SWFACTION_STRICTEQUALS            = 0x66,
  SWFACTION_GREATER                 = 0x67,
  SWFACTION_STRINGGREATER           = 0x68,

/* v7 actions */
  SWFACTION_DEFINEFUNCTION2         = 0x8E,
  SWFACTION_EXTENDS                 = 0x69,
  SWFACTION_TRY                     = 0x8F,
  SWFACTION_THROW                   = 0x2A,
  SWFACTION_CASTOP                  = 0x2B,
  SWFACTION_IMPLEMENTSOP            = 0x2C,
  SWFACTION_FSCOMMAND2              = 0x2D

} Action;



typedef enum
{
  PROPERTY_X              = 0x00,
  PROPERTY_Y              = 0x01,
  PROPERTY_XSCALE         = 0x02,
  PROPERTY_YSCALE         = 0x03,
  PROPERTY_CURRENTFRAME   = 0x04,
  PROPERTY_TOTALFRAMES    = 0x05,
  PROPERTY_ALPHA          = 0x06,
  PROPERTY_VISIBLE        = 0x07,
  PROPERTY_WIDTH          = 0x08,
  PROPERTY_HEIGHT         = 0x09,
  PROPERTY_ROTATION       = 0x0a,
  PROPERTY_TARGET         = 0x0b,
  PROPERTY_FRAMESLOADED   = 0x0c,
  PROPERTY_NAME           = 0x0d,
  PROPERTY_DROPTARGET     = 0x0e,
  PROPERTY_URL            = 0x0f,
  PROPERTY_HIGHQUALITY    = 0x10,
  PROPERTY_FOCUSRECT      = 0x11,
  PROPERTY_SOUNDBUFTIME   = 0x12,
  PROPERTY_QUALITY        = 0x13,
  PROPERTY_XMOUSE         = 0x14,
  PROPERTY_YMOUSE         = 0x15,
  PROPERTY_WTHIT          = 0x16	// not documented ???
} Property;

#define SWF_SETPROPERTY_X               0x0000
#define SWF_SETPROPERTY_Y               0x3F80
#define SWF_SETPROPERTY_XSCALE          0x4000
#define SWF_SETPROPERTY_YSCALE          0x4040
#define SWF_SETPROPERTY_ALPHA           0x40C0
#define SWF_SETPROPERTY_VISIBILITY      0x40E0
#define SWF_SETPROPERTY_ROTATION        0x4120
#define SWF_SETPROPERTY_NAME            0x4140
#define SWF_SETPROPERTY_HIGHQUALITY     0x4180
#define SWF_SETPROPERTY_SHOWFOCUSRECT   0x4188
#define SWF_SETPROPERTY_SOUNDBUFFERTIME 0x4190
#define SWF_SETPROPERTY_WTHIT           0x4680

#define DUPCLIP_NUMBER           0x4000

#endif /* SWF_ACTIONTYPES_H_INCLUDED */
