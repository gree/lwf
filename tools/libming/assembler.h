/* assembler.h
 * 
 * $Id$
 * 
 * Notice: This header file contains declarations of functions and types that
 * are just used internally. All library functions and types that are supposed
 * to be publicly accessable are defined in ./src/ming.h.
 */

#ifndef SWF_ASSEMBLER_H_INCLUDED
#define SWF_ASSEMBLER_H_INCLUDED

#include "ming.h"
#include "compile.h"

extern Buffer asmBuffer;

void bufferPatchLength(Buffer buffer, int len);
int bufferBranchTarget(Buffer buffer, char *label);

#endif /* SWF_ASSEMBLER_H_INCLUDED */
