import sys
import operator
import json
import re
splashArt = """
                                             ...,,,...                                              
                                   ..,,,,,,,,,,****,,,,,,,,,.                                       
                              .,,,**&@@@@@@@@@@@@@@@@@@@@@@@@&,.,,.                                 
                          .,,,/@@@@@@@@@@@@@&##&@@@@@@@@@@@@@@@@@@@,,,.                             
                        .,,&@@@@@@@@@@@@@@/******************,,,,..,,,,,,,***/////(((((//...        
                      ,**@@@@@@(*/@@@@@@@&,@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@* ...             
                     ,/(@@@@@*//***,*@@@@@.%@@@@@@@@@@@@@@@@@@@@@@@@@@@@@%*...                      
                    .**@@@@@@***,,,,,@@@@@#.%&&&&@@@@@@@@@@@@@@@#*,,.*%@@@(                         
                  .,,*,@@@@@@@@*,,(@@@@@@@@@..&&&&&@@&#,...,*#@@@@@@@@@@@@@/ *&&&&&&&&%#/,          
               .,*@@@@,,@@@@@@@@@@@@@@@@@@@@@@,........&@@@@@@@@@@@@@@@@@@&&*                       
             ,.@@@@@@@@&**/@@@@@&&&@@@@@@@@@@@/,@@&#&@@@@@@@@@@@@@@@@@@@@@&&&,                      
          .,*@@@@@@@@@@@@@@/,..............,/%.*../@@@@@@@@@@@@@@@@@@@@@@@&&&&.                     
        ,*#@@@@@@@@@@@@@@@,.*&&&&&.,&&&&&&&&&.((((*..#@@@@@@@@@@@@@@@@@@@@@&&&&                     
       ..@@@@@@@@@@@(.,./@@&&&&&&(.@@@@@@@@@#.%%%#(*.*..&@@@@@@@@@@@@@@@@@@@@&&&                    
            .*/(#&@@./.,&&&&&&&&&.*@@@@@@@@@@.%%%%%*.@&...,&@@@@@@@@@@@@@@@@@@&&&                   
           .@@@@@@@,.(,.&&&&&&&&&.&@@@@@@@@@@%.%%%%(....(((...&@@@@@@@@@@@@@@@@&&#                  
         .,@@@@@@& .//* &&&&&&&&&.*,.*(@@@@@@@@.*%%%(,/%%%#(((...@@@@@@@@@@@@@@@&&*                 
        ..@@&&&&. *///* (&&&&&&&@.*///(/..@@@@@@@%.,#%%%%%%%%((/....@@@@@@@@@@@@@&&,                
         %&&&%    ,**** ,%&&&&&&@,.///////.*@@@@@@@@@@@#/**(%@@@&&&&...&@@@@@@@@@&&&.               
        .&/        ****. %%%&&&@@@.*///////.........,@@@@@@@@@@@@@@&&&....(@@@@@@@&&&               
                   .**** (%%%%%@@@@.,//////////////*.&@@@@@@@@@@@@@@@#.......,@@@&&&&/              
                     ***. %##%%@@@@@,./////////////(./@@@@@@%/@@@@@@@@&          %&&&&              
                       ** (####&&&@@@@*.,///////////,....,///, %&&&&&&&&&&&&&####.  .&&             
                           ####&&&&&&@@@&..*/(%%###((%%%%(//**  #&&&&&&&&&&&&&&&%.                  
                            ,##&&&&&&&&&&@@@#%&@@@@.,%%%%%###./&(      ,&&&&&&&/                    
                               .&&&&&&&&&&&&@@@@@@@.(######(/ &&&        (&&&*                      
                                  .&&&&&&&&&&&&@@@@.,####### %@&@          .                        
                                      %&&&&&&&&&&&&@ .##### *&&@@&&&%.                              
                                         (&&&&&&&&&&&&*  ...&&&&&&&&&&.                             
                                            *&&&&&&&&&&&&&%*.. *(&/                                 
                                                &&&&&&&&&&&*.                                       

                                  ___           ___           ___           ___                        
                                 /\  \         /\  \         /\  \         /\  \                       
                                /::\  \       /::\  \       /::\  \       /::\  \                      
                               /:/\:\  \     /:/\:\  \     /:/\:\  \     /:/\:\  \                     
                              /::\~\:\  \   /:/  \:\  \   /::\~\:\  \   /:/  \:\  \                    
                             /:/\:\ \:\__\ /:/__/_\:\__\ /:/\:\ \:\__\ /:/__/ \:\__\                   
                             \/__\:\/:/  / \:\  /\ \/__/ \/_|::\/:/  / \:\  \ /:/  /                   
                                  \::/  /   \:\ \:\__\      |:|::/  /   \:\  /:/  /                    
                                  /:/  /     \:\/:/  /      |:|\/__/     \:\/:/  /                     
                                 /:/  /       \::/  /       |:|  |        \::/  /                      
                                 \/__/         \/__/         \|__|         \/__/                       

                                                  AGRO® 2021
"""
class MemoryContext:
    def __init__(self):
        self.int    = []
        self.float  = []
        self.char   = []
        self.string = []
    
    def allocateSpace(self, intCount, floatCount, charCount, stringCount):
        self.int.extend([None for _ in range(intCount)])
        self.float.extend([None for _ in range(floatCount)])
        self.char.extend([None for _ in range(charCount)])
        self.string.extend([None for _ in range(stringCount)])

    def setInt(self, index, value):
        self.int[index] = int(value)

    def setFloat(self, index, value):
        self.float[index] = float(value)

    def setChar(self, index, value):
        self.char[index] = value

    def setString(self, index, value):
        self.string[index] = value

    def getInt(self, index):
        return self.int[index]

    def getFloat(self, index):
        return self.float[index]

    def getChar(self, index):
        return self.char[index]

    def getString(self, index):
        return self.string[index]

class Memory:
    def __init__(self):
        # Many MemoryContext
        self.localMemory = []

        # Many Temp MemoryContext
        self.localTempMemory = []

        # Global Memory
        self.globalMemory   = MemoryContext()

        # Global Temp Memory
        self.globalTempMemory   = MemoryContext()
        
        # Constant Global Memory
        self.constantMemory = MemoryContext()

        # Pointers Global Memory
        self.pointersMemory = MemoryContext()

        # Global variables lower limit
        self.globalInt = 1001
        self.globalFloat = 5001
        self.globalChar = 9001
        self.globalTempInt = 12001
        self.globalTempFloat = 16001
        self.globalTempChar = 20001
        self.globalTempString = 24001

        # Local variables lower limit
        self.localInt = 28001
        self.localFloat = 30001
        self.localChar = 32001
        self.localTempInt = 34001
        self.localTempFloat = 36001
        self.localTempChar = 38001
        self.localTempString = 40001

        # Constant variables lower limit
        self.constInt = 42001
        self.constFloat = 44001
        self.constChar = 46001
        self.constString = 48001

        # Pointers lower limit
        self.pointersMem = 50001

    def allocateLocalMemory(self, intCount, floatCount, charCount, stringCount):
        self.localMemory.append(MemoryContext())
        self.localMemory[-1].allocateSpace(intCount, floatCount, charCount, stringCount)

    def allocateLocalTempMemory(self, intCount, floatCount, charCount, stringCount):
        self.localTempMemory.append(MemoryContext())
        self.localTempMemory[-1].allocateSpace(intCount, floatCount, charCount, stringCount)

    def allocateGlobalMemory(self, intCount, floatCount, charCount, stringCount):
        self.globalMemory.allocateSpace(intCount, floatCount, charCount, stringCount)

    def allocateGlobalTempMemory(self, intCount, floatCount, charCount, stringCount):
        self.globalTempMemory.allocateSpace(intCount, floatCount, charCount, stringCount)

    def allocateConstantMemory(self, intCount, floatCount, charCount, stringCount):
        self.constantMemory.allocateSpace(intCount, floatCount, charCount, stringCount)

    def allocatePointersMemory(self, intCount, floatCount, charCount, stringCount):
        self.pointersMemory.allocateSpace(intCount, floatCount, charCount, stringCount)

    def getOrderedDir(self):
        "Matches memory dir with actual memory structures"
        # Not efficient. Try returning methods to get or set values
        return [
            ("int",         self.globalInt,         self.globalMemory.getInt,           self.globalMemory.setInt),
            ("float",       self.globalFloat,       self.globalMemory.getFloat,         self.globalMemory.setFloat),
            ("char",        self.globalChar,        self.globalMemory.getChar,          self.globalMemory.setChar),

            ("int",         self.globalTempInt,     self.globalTempMemory.getInt,       self.globalTempMemory.setInt),
            ("float",       self.globalTempFloat,   self.globalTempMemory.getFloat,     self.globalTempMemory.setFloat),
            ("char",        self.globalTempChar,    self.globalTempMemory.getChar,      self.globalTempMemory.setChar),
            ("string",      self.globalTempString,  self.globalTempMemory.getString,    self.globalTempMemory.setString),
            
            ("int",         self.localInt,          self.localMemory[-1].getInt,        self.localMemory[-1].setInt),
            ("float",       self.localFloat,        self.localMemory[-1].getFloat,      self.localMemory[-1].setFloat),
            ("char",        self.localChar,         self.localMemory[-1].getChar,       self.localMemory[-1].setChar),

            ("int",         self.localTempInt,      self.localTempMemory[-1].getInt,    self.localTempMemory[-1].setInt),
            ("float",       self.localTempFloat,    self.localTempMemory[-1].getFloat,  self.localTempMemory[-1].setFloat),
            ("char",        self.localTempChar,     self.localTempMemory[-1].getChar,   self.localTempMemory[-1].setChar),
            ("string",      self.localTempString,   self.localTempMemory[-1].getString, self.localTempMemory[-1].setString),

            ("int",         self.constInt,          self.constantMemory.getInt,         self.constantMemory.setInt),
            ("float",       self.constFloat,        self.constantMemory.getFloat,       self.constantMemory.setFloat),
            ("char",        self.constChar,         self.constantMemory.getChar,        self.constantMemory.setChar),
            ("string",      self.constString,       self.constantMemory.getString,      self.constantMemory.setString),

            ("int",         self.pointersMem,       self.constantMemory.getInt,         self.constantMemory.setInt)
        ]

    def getValue(self, address):
        "Iterate through memory address limits to find appropiate memory segment offset and get value"
        # ("orderedDirName", lower_limit_address)
        outputTuple = ()

        # Find lower limit and return tuple with memory space
        for limitTuple in self.getOrderedDir():
            if (address < limitTuple[1]):
                break
            outputTuple = limitTuple
        
        # Return offset value in memory. Normalized to 0
        return outputTuple[2](address - outputTuple[1])
    
    def setValue(self, address, value):
        "Iterate through memory address limits to find appropiate memory segment offset and set value"
        # ("orderedDirName", lower_limit_address)
        outputTuple = ()

        for limitTuple in self.getOrderedDir():
            if (address < limitTuple[1]):
                break
            outputTuple = limitTuple
        
        # Sets offset value in memory. Normalized to 0
        outputTuple[3](address - outputTuple[1], value)

    def setValueOnPointer(self, originPointer, pointerToPointer):
        # ("orderedDirName", lower_limit_address)
        outputTuple = ()

        for limitTuple in self.getOrderedDir():
            if (pointerToPointer < limitTuple[1]):
                break
            outputTuple = limitTuple
        
        # Sets offset value in memory. Normalized to 0
        outputTuple[3](self.getValue(pointerToPointer), self.getValue(originPointer))

    def pushContext(self):
        "Add new level on local memory context"
        self.localMemory.push(MemoryContext())
        self.localTempMemory.push(MemoryContext())

    def popContext(self):
        "Erase top most level on local memory"
        self.localMemory.pop()
        self.localTempMemory.pop()

class CodeProcessor:
    def __init__(self, code, cube, dirFunc, constants, debug):
        self.code = code
        self.cube = cube
        self.dirFunc = dirFunc
        self.constants = constants
        self.debug = debug
        self.memory = Memory()
        self.codeLine = 0
        self.jumpStack = []
        self.paramStack = []
        self.instructions = {
            "+":        self.simpleOperator,
            "-":        self.simpleOperator,
            "/":        self.simpleOperator,
            "*":        self.simpleOperator,
            ">":        self.simpleOperator,
            "<":        self.simpleOperator,
            "=":        self.assign,
            "goto":     self.goto,
            "gotoFalse":self.gotoFalse,
            "gotoTrue": self.gotoTrue,
            "print":    self._print,
            "era":      self.test,
            "param":    self.param,
            "goSub":    self.gosub,
            "endfunc":  self.endfunc,
            "return":   self._return,
            "input":    self._input,
            "verify":   self.verify
        }

        self.useDirFuncToAllocateInMethod("_global", self.memory.allocateGlobalMemory)
        self.useDirFuncToAllocateTmpInMethod("_global", self.memory.allocateGlobalTempMemory)

        self.useDirFuncToAllocateInMethod("_main", self.memory.allocateLocalMemory)
        self.useDirFuncToAllocateTmpInMethod("_main", self.memory.allocateLocalTempMemory)

        # Initialize constant memory
        c_int = 0
        c_float = 0
        c_char = 0
        c_string = 0
        orderedDir = self.memory.getOrderedDir()
        listToSet = []
        for line in constants:
            if len(line) < 2: break
            # [ DIR VALUE ]
            line = [int(line[0]), line[1]]
            # ("orderedDirName", lower_limit_address)
            outputTuple = ()

            # Find lower limit and get tuple with memory space
            for limitTuple in orderedDir:
                if (line[0] < limitTuple[1]):
                    break
                outputTuple = limitTuple

            if (outputTuple[1] == self.memory.constInt):
                c_int += 1
            elif (outputTuple[1] == self.memory.constFloat):
                c_float += 1
            elif (outputTuple[1] == self.memory.constChar):
                c_char += 1
                # Erase quote
                line[1] = line[1][1:-1]
            elif (outputTuple[1] == self.memory.constString):
                c_string += 1
                # Erase quote
                line[1] = line[1][1:-1]

            listToSet.append((line, outputTuple))

        self.memory.allocateConstantMemory(c_int, c_float, c_char, c_string)

        # Set values to constant memory
        for varToSet in listToSet:
            print(varToSet)
            varToSet[1][3](int(varToSet[0][0]) - varToSet[1][1], varToSet[0][1])

    def consoleLog(self, text):
        "Uses debug flag to print or not debug text"
        if self.debug:
            print(text)

    def toBool(self, value):
        "Bool representation from AGRO to python"
        return int(value) == 1

    def verify(self, quadArray):
        "Verfies array dimensions"
        tmpIndex = self.memory.getValue(int(quadArray[1]))
        self.consoleLog(tmpIndex)
        if tmpIndex < 0 or tmpIndex > int(quadArray[2]):
            print("Array index out of bounds")
            # Go to final execution line to end program
            self.codeLine = len(self.code)
        else:
            # Move to next instruction
            self.codeLine += 1    

    def _input(self, quadArray):
        "Inputs to str dir"
        self.memory.setValue(int(quadArray[1]), input())

        # Move to next instruction
        self.codeLine += 1

    def _return(self, quadArray):
        "Assigns return value to global return variable named the same as function"
        self.assign(quadArray)

    def endfunc(self, quadArray):
        "Pops local memory context and pop jump stack to return to program execution"
        self.memory.popContext()
        self.codeLine = self.jumpStack.pop()[1]

    def param(self, quadArray):
        "Append to param stack to use on gosub"
        self.paramStack.append(self.memory.getValue(int(quadArray[1])))
        # Move to next instruction
        self.codeLine += 1

    def useDirFuncToAllocateInMethod(self, funcName, method):
        # allocateLocalMemory(intCount, floatCount, charCount, stringCount)
        method(
            self.dirFunc[funcName]["int"],
            self.dirFunc[funcName]["float"],
            self.dirFunc[funcName]["char"],
            self.dirFunc[funcName]["string"]
        )

    def useDirFuncToAllocateTmpInMethod(self, funcName, method):
        # allocateLocalTempMemory(intCount, floatCount, charCount, stringCount)
        method(
            self.dirFunc[funcName]["intTmp"],
            self.dirFunc[funcName]["floatTmp"],
            self.dirFunc[funcName]["charTmp"],
            self.dirFunc[funcName]["stringTmp"]
        )

    def era(self, quadArray):
        # Move to next instruction
        self.codeLine += 1

    def eraObject(self, quadArray):
        '''
            MemoryContext SymbolTable local super local int float char string
            lucas = MemoryContext [0,1,2,3] pointers
            goMethod en vez de crear un MemoryContext, solo se lo paso
            lucas ? direcciones en la sTable actual?
        '''

    def processCode(self):
        "Iterates through code lines and execute processor instructions"
        codeLen = len(self.code) - 1
        while(self.codeLine < codeLen):
            line = self.code[self.codeLine]
            self.consoleLog("----------------------------------------")
            self.consoleLog(line)
            line = line.split()
            self.instructions[line[0]](line)
    
    def test(self, quadArray):
        # Move to next instruction
        self.codeLine += 1

    def goto(self, quadArray):
        "Moves to code line"
        self.codeLine = int(quadArray[1])

    def gotoFalse(self, quadArray):
        "Moves to code line if false"
        if self.toBool(self.memory.getValue(int(quadArray[1]))) == False:
            self.codeLine = int(quadArray[2])
        else:
            self.codeLine += 1

    def gotoTrue(self, quadArray):
        "Moves to code line if true"
        if self.toBool(self.memory.getValue(int(quadArray[1]))) == True:
            self.codeLine = int(quadArray[2])
        else:
            self.codeLine += 1

    def gosub(self, quadArray):
        self.useDirFuncToAllocateInMethod(quadArray[1], self.memory.allocateLocalMemory)
        self.useDirFuncToAllocateTmpInMethod(quadArray[1], self.memory.allocateLocalTempMemory)

        # Use params as values
        paramTypes = self.dirFunc[quadArray[1]]["params"]
        paramValues = []
        # Pop params values from param stack depending on dirFunc param count
        for _ in paramTypes:
            paramValues.append(self.paramStack.pop())
        # Reverse params from stack to queue
        paramValues.reverse()

        # First vars from function are assigned to local memory
        intCount = 0
        floatCount = 0
        charCount = 0
        for i in range(len(paramTypes)):
            if paramTypes[i] == "int":
                self.memory.setValue(self.memory.localInt + intCount, paramValues[i])
                intCount += 1
            elif paramTypes[i] == "float":
                self.memory.setValue(self.memory.localFloat + floatCount, paramValues[i])
                floatCount += 1
            elif paramTypes[i] == "char":
                self.memory.setValue(self.memory.localChar + charCount, paramValues[i])
                charCount += 1
            
        # Save function namd and next instruction pointer to continue program after endfunc
        self.jumpStack.append((quadArray[1], self.codeLine + 1))
        # Move to next instruction
        self.codeLine = self.dirFunc[quadArray[1]]['index']

    def goMethod(self, quadArray):
        '''
        [45678] = pointer int 1
        [21343] = float 2.34123

            1. Allocate method memory, locales y temp del método (parámetros también)
            2. Asignar valor de parámetros del objeto en orden a la nueva memoria desde la memoria vieja
            3. Asignar valor de parámetros en orden
            4. (12345)
        '''

    def assign(self, quadArray):
        "Assigns dir value to dir value"

        if int(quadArray[2]) >= self.memory.pointersMem:
            self.memory.setValueOnPointer(int(quadArray[1]), int(quadArray[2]))
        else:
            self.memory.setValue(
                int(quadArray[2]),
                self.memory.getValue(int(quadArray[1]))
            )
        
        # Move to next instruction
        self.codeLine += 1

    def _print(self, quadArray):
        "Prints dir value"
        print(self.memory.getValue(int(quadArray[1])))
        
        # Move to next instruction
        self.codeLine += 1

    # { + operatorLeft operatorRight output }
    def simpleOperator(self, quadArray):
        ops = {
            "+":    operator.add,
            "-":    operator.sub,
            "/":    operator.truediv,
            "*":    operator.mul,
            ">":    operator.gt,
            "<":    operator.lt
        }
        op = quadArray[0]
        lDir = int(quadArray[1])
        rDir = int(quadArray[2])
        lVal = self.memory.getValue(lDir)
        rVal = self.memory.getValue(rDir)
        dirOut = int(quadArray[3])
        self.consoleLog("{0} = {1} {2} {3}\n{0} = {4} {2} {5}".format(dirOut, lDir, op, rDir, lVal, rVal))

        self.memory.setValue(
            dirOut,
            ops[op](lVal, rVal)
        )

        # Move to next instruction
        self.codeLine += 1


if __name__ == "__main__":
    if len(sys.argv) < 2:
        print("Usage: python AGRO.py programName")
    else:

        # Use this filename extension to read output file from compiler
        extensionProgram = ".agro"
        extensionCode = extensionProgram + ".code"
        extensionCube = extensionProgram + ".cube"
        extensionDirFunc = extensionProgram + ".dirfunc"
        extensionConstants = extensionProgram + ".constants"

        # Debug flag
        debug = True

        # Program name without extension
        programName = sys.argv[1]

        # Read program file
        file = open(programName + extensionCode, 'r')
        code = file.read().split('\n')
        file.close()

        # Read cube program file
        file = open(programName + extensionCube, 'r')
        cube = json.load(file)
        file.close()

        if debug:
            print(cube)

        # Read dirFunc for program
        file = open(programName + extensionDirFunc, 'r')
        dirFuncTxt = file.read().split('\n')
        file.close()

        dirFunc = {}
        if debug:
            print(dirFuncTxt)
        for line in dirFuncTxt:
            if len(line) < 2: break
            if debug:
                print(line)
            line = line.split('|')
            name = line[0]
            params = line[1].split()
            for i in range(len(params)):
                # int = 1
                if params[i] == "1":
                    params[i] = "int"
                # float = 2
                elif params[i] == "2":
                    params[i] = "float"
                # char = 3
                elif params[i] == "3":
                    params[i] = "char"
                # string = 6
                elif params[i] == "6":
                    params[i] = "string"

            line = line[2].split()
            dirFunc[name] = {
                "index":    int(line[0]),
                "params":   params,
                "int":      int(line[1]),
                "float":    int(line[2]),
                "char":     int(line[3]),
                "string":   int(line[4]),
                "intTmp":   int(line[5]),
                "floatTmp": int(line[6]),
                "charTmp":  int(line[7]),
                "stringTmp":int(line[8])
            }

        if debug:
            print(dirFunc)

        # Read constants for program
        file = open(programName + extensionConstants, 'r')
        constants = file.read()
        file.close()

        constants = re.findall(r"(\d{5})\s(.*)\n", constants)

        agroVM = CodeProcessor(code, cube, dirFunc, constants, debug)
        print(splashArt)
        agroVM.processCode()
