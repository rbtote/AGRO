import sys
import operator
import re

# AGRO splash art displayed at start of program
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
        "Appends N intCount, floatCount, charCount, stringCount to memory arrays"
        self.int.extend([None for _ in range(intCount)])
        self.float.extend([None for _ in range(floatCount)])
        self.char.extend([None for _ in range(charCount)])
        self.string.extend([None for _ in range(stringCount)])

    # Setters

    def setInt(self, index, value):
        self.int[index] = (int(value) if value is not None else None) 

    def setFloat(self, index, value):
        self.float[index] = (float(value) if value is not None else None)

    def setChar(self, index, value):
        self.char[index] = value

    def setString(self, index, value):
        self.string[index] = value

    # Getters

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
        self.globalInt = 0
        self.globalFloat = 2000
        self.globalChar = 4000
        self.globalString = 6000
        self.globalTempInt = 8000
        self.globalTempFloat = 9000
        self.globalTempChar = 10000
        self.globalTempString = 11000

        # Local variables lower limit
        self.localInt = 12000
        self.localFloat = 16000
        self.localChar = 20000
        self.localString = 24000
        self.localTempInt = 28000
        self.localTempFloat = 32000
        self.localTempChar = 36000
        self.localTempString = 40000

        # Constant variables lower limit
        self.constInt = 44000
        self.constFloat = 46000
        self.constChar = 48000
        self.constString = 50000

        # Pointers lower limit
        self.pointersMem = 52000

    def allocateLocalMemory(self, intCount, floatCount, charCount, stringCount):
        "Abstraction of allocateSpace method for local memory"
        self.localMemory.append(MemoryContext())
        self.localMemory[-1].allocateSpace(intCount, floatCount, charCount, stringCount)

    def allocateLocalTempMemory(self, intCount, floatCount, charCount, stringCount):
        "Abstraction of allocateSpace method for local temp memory"
        self.localTempMemory.append(MemoryContext())
        self.localTempMemory[-1].allocateSpace(intCount, floatCount, charCount, stringCount)

    def allocateGlobalMemory(self, intCount, floatCount, charCount, stringCount):
        "Abstraction of allocateSpace method for global memory"
        self.globalMemory.allocateSpace(intCount, floatCount, charCount, stringCount)

    def allocateGlobalTempMemory(self, intCount, floatCount, charCount, stringCount):
        "Abstraction of allocateSpace method for global temp memory"
        self.globalTempMemory.allocateSpace(intCount, floatCount, charCount, stringCount)

    def allocateConstantMemory(self, intCount, floatCount, charCount, stringCount):
        "Abstraction of allocateSpace method for constant memory"
        self.constantMemory.allocateSpace(intCount, floatCount, charCount, stringCount)

    def allocatePointersMemory(self, intCount, floatCount, charCount, stringCount):
        "Abstraction of allocateSpace method for pointers memory"
        self.pointersMemory.allocateSpace(intCount, floatCount, charCount, stringCount)

    def getOrderedDir(self):
        "Matches ordered memory offsets with var type, and getter/setter methods for actual memory structures"
        return [
            ("int",         self.globalInt,         self.globalMemory.getInt,           self.globalMemory.setInt),
            ("float",       self.globalFloat,       self.globalMemory.getFloat,         self.globalMemory.setFloat),
            ("char",        self.globalChar,        self.globalMemory.getChar,          self.globalMemory.setChar),
            ("string",      self.globalString,      self.globalMemory.getString,        self.globalMemory.setString),

            ("int",         self.globalTempInt,     self.globalTempMemory.getInt,       self.globalTempMemory.setInt),
            ("float",       self.globalTempFloat,   self.globalTempMemory.getFloat,     self.globalTempMemory.setFloat),
            ("char",        self.globalTempChar,    self.globalTempMemory.getChar,      self.globalTempMemory.setChar),
            ("string",      self.globalTempString,  self.globalTempMemory.getString,    self.globalTempMemory.setString),
            
            ("int",         self.localInt,          self.localMemory[-1].getInt,        self.localMemory[-1].setInt),
            ("float",       self.localFloat,        self.localMemory[-1].getFloat,      self.localMemory[-1].setFloat),
            ("char",        self.localChar,         self.localMemory[-1].getChar,       self.localMemory[-1].setChar),
            ("string",      self.localString,       self.localMemory[-1].getString,     self.localMemory[-1].setString),

            ("int",         self.localTempInt,      self.localTempMemory[-1].getInt,    self.localTempMemory[-1].setInt),
            ("float",       self.localTempFloat,    self.localTempMemory[-1].getFloat,  self.localTempMemory[-1].setFloat),
            ("char",        self.localTempChar,     self.localTempMemory[-1].getChar,   self.localTempMemory[-1].setChar),
            ("string",      self.localTempString,   self.localTempMemory[-1].getString, self.localTempMemory[-1].setString),

            ("int",         self.constInt,          self.constantMemory.getInt,         self.constantMemory.setInt),
            ("float",       self.constFloat,        self.constantMemory.getFloat,       self.constantMemory.setFloat),
            ("char",        self.constChar,         self.constantMemory.getChar,        self.constantMemory.setChar),
            ("string",      self.constString,       self.constantMemory.getString,      self.constantMemory.setString),

            ("int",         self.pointersMem,       self.pointersMemory.getInt,         self.pointersMemory.setInt)
        ]

    def getType(self, address):
        "Iterate through memory address limits to find appropiate memory segment offset and get value"
        # (var_type, lower_limit_address, getter, setter)
        outputTuple = ()

        # Find lower limit and return tuple with memory space
        for limitTuple in self.getOrderedDir():
            if (address < limitTuple[1]):
                break
            outputTuple = limitTuple

        return outputTuple[0]

    def getValue(self, address, usePointerOut=True):
        "Iterate through memory address limits to find appropiate memory segment offset and get value"
        # (var_type, lower_limit_address, getter, setter)
        outputTuple = ()

        # Find lower limit and return tuple with memory space
        for limitTuple in self.getOrderedDir():
            if (address < limitTuple[1]):
                break
            outputTuple = limitTuple
        
        if usePointerOut and outputTuple[1] == self.pointersMem:
            return self.getValue(outputTuple[2](address - outputTuple[1]))
        else:
            # Return offset value in memory. Normalized to 0
            return outputTuple[2](address - outputTuple[1])
    
    def setValue(self, address, value, usePointerOut=False):
        "Iterate through memory address limits to find appropiate memory segment offset and set value"
        # (var_type, lower_limit_address, getter, setter)
        outputTuple = ()

        # Find lower limit and return tuple with memory space
        for limitTuple in self.getOrderedDir():
            if (address < limitTuple[1]):
                break
            outputTuple = limitTuple
        
        if usePointerOut and outputTuple[1] == self.pointersMem:
            # Sets offset value in memory. Normalized to 0
            self.setValue(self.getValue(address, usePointerOut=False), value, False)
        else:
            # Sets offset value in memory. Normalized to 0
            outputTuple[3](address - outputTuple[1], value)

    def pushContext(self):
        "Add new level on local memory context"
        self.localMemory.push(MemoryContext())
        self.localTempMemory.push(MemoryContext())

    def popContext(self):
        "Erase top most level on local memory"
        self.localMemory.pop()
        self.localTempMemory.pop()

class CodeProcessor:
    def __init__(self, programName, debug):

        self.programName = programName
        self.debug = debug

        # AGRO first extension
        extensionProgram = ".agro"

        # AGRO second extension for generated Code file
        extensionCode = extensionProgram + ".code"

        # AGRO second extension for generated Function Directory file
        extensionDirFunc = extensionProgram + ".dirfunc"

        # AGRO second extension for generated Constants file
        extensionConstants = extensionProgram + ".constants"

        # AGRO second extension for generated Class file
        extensionClasses = extensionProgram + ".classes";

        # Read agro.code file. Split by \n into array
        file = open(self.programName + extensionCode, 'r')
        self.code = file.read().split('\n')
        file.close()

        # Read agro.classes file. Split by \n into array
        file = open(self.programName + extensionClasses, 'r')
        classesTxt = file.read().split('\n')
        file.close()

        '''
            Process classes file into structure
            classes["class_name"] = {
                "int":      int count,
                "float":    float count,
                "char":     char count,
                "string":   string count
            }
        '''
        self.classes = {}
        for line in classesTxt:
            if len(line) < 2: break
            line = line.split()
            self.classes[line[0]] = {
                "int":      int(line[1]),
                "float":    int(line[2]),
                "char":     int(line[3]),
                "string":   int(line[4])
            }

        # Read agro.dirfunc file. Split by \n into array
        file = open(self.programName + extensionDirFunc, 'r')
        dirFuncTxt = file.read().split('\n')
        file.close()

        '''
            Process dirfunc into structure
            dirFunc["function_name"] = {
                "index":    Code Index of function,
                "params":   Param Types Array,
                "int":      int count,
                "float":    float count,
                "char":     char count,
                "string":   string count,
                "intTmp":   int temp count,
                "floatTmp": float temp count,
                "charTmp":  char temp count,
                "stringTmp":string temp count
            }
        '''
        self.dirFunc = {}
        for line in dirFuncTxt:
            if len(line) < 2: break
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
            self.dirFunc[name] = {
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

        '''
            Read constants for program into constant array using regex
            [(dir, value)]
        '''
        file = open(self.programName + extensionConstants, 'r')
        self.constants = file.read()
        file.close()
        self.constants = re.findall(r"(\d{5})\s(.*)\n", self.constants)

        # Memory for the program
        self.memory = Memory()
        
        # Current code line
        self.codeLine = 0

        # Jump stack used to return to previous code line after calls
        self.jumpStack = []

        # Param stack used to call functions
        self.paramStack = []

        # Param stack used to call object methods
        self.objectParamStack = []

        # Stack used to return object states
        self.objectReturnDir = []

        # Actual processor instructions map to function handlers 
        self.instructions = {
            "+":        self.simpleOperator,
            "-":        self.simpleOperator,
            "/":        self.simpleOperator,
            "//":        self.simpleOperator,
            "*":        self.simpleOperator,
            ">":        self.simpleOperator,
            "<":        self.simpleOperator,
            "=":        self.assign,
            "==":       self.simpleOperator,
            "!=":       self.simpleOperator,
            "<=":       self.simpleOperator,
            ">=":       self.simpleOperator,
            "++":       self.add1,
            "+=":       self.addEqual,
            "--":       self.sub1,
            "-=":       self.subEqual,
            "*=":       self.mulEqual,
            "/=":       self.divEqual,
            "&&":       self.simpleOperator,
            "||":       self.simpleOperator,
            "%":        self.simpleOperator,
            "**":       self.simpleOperator,
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
            "verify":   self.verify,
            "object":   self._object
        }

        # Processor simple operator methods
        self.simpleOperators = {
            "+":    operator.add,
            "-":    operator.sub,
            "/":    operator.truediv,
            "//":   operator.floordiv,
            "*":    operator.mul,
            "**":   operator.pow,
            ">":    operator.gt,
            "<":    operator.lt,
            "==":   operator.eq,
            "!=":   operator.ne,
            "<=":   operator.le,
            ">=":   operator.ge,
            "%":    operator.mod,
            "&&":   self._and,
            "||":   self._or
        }

        # Allocate Global memory from dirfunc _global
        self.useDirFuncToAllocateInMethod("_global", self.memory.allocateGlobalMemory)
        self.useDirFuncToAllocateTmpInMethod("_global", self.memory.allocateGlobalTempMemory)

        # Allocate Main memory from dirfunc _main
        self.useDirFuncToAllocateInMethod("_main", self.memory.allocateLocalMemory)
        self.useDirFuncToAllocateTmpInMethod("_main", self.memory.allocateLocalTempMemory)

        # Allocate Pointer memory from dirfunc _pointer
        self.useDirFuncToAllocateInMethod("_pointer", self.memory.allocatePointersMemory)

        # Initialize constant memory
        # Count how many vars are needed to allocate
        c_int = 0
        c_float = 0
        c_char = 0
        c_string = 0
        orderedDir = self.memory.getOrderedDir()
        listToSet = []
        for line in self.constants:

            # Jump empty lines
            if len(line) < 2: continue
            
            # [ DIR VALUE ]
            line = [int(line[0]), line[1]]

            # (var_type, lower_limit_address, getter, setter)
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
            elif (outputTuple[1] == self.memory.constString):
                c_string += 1
                # Erase string quote
                line[1] = line[1][1:-1]

            # Append current constant and dir reference to listToSet
            listToSet.append((line, outputTuple))

        # Allocate Constant memory from constant count
        self.memory.allocateConstantMemory(c_int, c_float, c_char, c_string)

        # Set each saved constant value to Constant memory
        for varToSet in listToSet:
            varToSet[1][3](int(varToSet[0][0]) - varToSet[1][1], varToSet[0][1])

    def _object(self, quadArray):
        "object param quad that appends parameter to call a function"
        self.objectParamStack.append(int(quadArray[1]))
        # Move to next instruction
        self.codeLine += 1

    def divEqual(self, quadArray):
        "operator /="
        self.memory.setValue(
            int(quadArray[2]),
            self.memory.getValue(int(quadArray[2])) / self.memory.getValue(int(quadArray[1])),
            usePointerOut=False
        )
        # Move to next instruction
        self.codeLine += 1

    def mulEqual(self, quadArray):
        "operator *="
        self.memory.setValue(
            int(quadArray[2]),
            self.memory.getValue(int(quadArray[2])) * self.memory.getValue(int(quadArray[1])),
            usePointerOut=False
        )
        # Move to next instruction
        self.codeLine += 1

    def subEqual(self, quadArray):
        "operator -="
        self.memory.setValue(
            int(quadArray[2]),
            self.memory.getValue(int(quadArray[2])) - self.memory.getValue(int(quadArray[1])),
            usePointerOut=False
        )
        # Move to next instruction
        self.codeLine += 1

    def addEqual(self, quadArray):
        "operator +="
        self.memory.setValue(
            int(quadArray[2]),
            self.memory.getValue(int(quadArray[2])) + self.memory.getValue(int(quadArray[1])),
            usePointerOut=False
        )
        # Move to next instruction
        self.codeLine += 1

    def _or(self, lVal, rVal):
        "Mimics operator ||"
        return lVal or rVal;

    def _and(self, lVal, rVal):
        "Mimics operator &&"
        return lVal and rVal;
        

    def sub1(self, quadArray):
        "operator --"
        self.memory.setValue(
            int(quadArray[1]),
            self.memory.getValue(int(quadArray[1])) - 1,
            usePointerOut=False
        )
        # Move to next instruction
        self.codeLine += 1

    def add1(self, quadArray):
        "operator ++"
        self.memory.setValue(
            int(quadArray[1]),
            self.memory.getValue(int(quadArray[1])) + 1,
            usePointerOut=False
        )
        # Move to next instruction
        self.codeLine += 1

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
        "Assigns return value to global return variable named the same as function. Uses self.assign instead"
        self.assign(quadArray)

    def endMethod(self, quadArray):
        "Ends method by returning object state to previous MemoryContext"
        
        currentObjectReturnDirs = self.objectReturnDir.pop()

        values = []

        # Ordered values from inner memory are saved into tmp array
        intCount = 0
        floatCount = 0
        charCount = 0
        stringCount = 0

        for i in range(len(currentObjectReturnDirs)):
            if currentObjectReturnDirs[i][0] == "int":
                values.append(self.memory.getValue(self.memory.localInt + intCount))
                intCount += 1
            elif currentObjectReturnDirs[i][0] == "float":
                values.append(self.memory.getValue(self.memory.localFloat + floatCount))
                floatCount += 1
            elif currentObjectReturnDirs[i][0] == "char":
                values.append(self.memory.getValue(self.memory.localChar + charCount))
                charCount += 1
            elif currentObjectReturnDirs[i][0] == "string":
                values.append(self.memory.getValue(self.memory.localString + stringCount))
                stringCount += 1

        # Drop method context and return to previous context
        self.memory.popContext()
        
        # Ordered vars from currentObjectReturnDirs are assigned to previous local memory
        intCount = 0
        floatCount = 0
        charCount = 0
        stringCount = 0

        for i in range(len(currentObjectReturnDirs)):
            if currentObjectReturnDirs[i][0] == "int":
                if values[i] is not None:
                    self.memory.setValue(currentObjectReturnDirs[i][1], values[i])
                intCount += 1
            elif currentObjectReturnDirs[i][0] == "float":
                if values[i] is not None:
                    self.memory.setValue(currentObjectReturnDirs[i][1], values[i])
                floatCount += 1
            elif currentObjectReturnDirs[i][0] == "char":
                if values[i] is not None:
                    self.memory.setValue(currentObjectReturnDirs[i][1], values[i])
                charCount += 1
            elif currentObjectReturnDirs[i][0] == "string":
                if values[i] is not None:
                    self.memory.setValue(currentObjectReturnDirs[i][1], values[i])
                stringCount += 1

        self.codeLine = self.jumpStack.pop()[1]


    def endfunc(self, quadArray):
        "Pops local memory context and pop jump stack to return to program execution"
        if '.' in self.jumpStack[-1][0]:
            self.endMethod(quadArray)
            return

        self.memory.popContext()
        self.codeLine = self.jumpStack.pop()[1]

    def param(self, quadArray):
        "Append to param stack to use on gosub"
        self.paramStack.append(self.memory.getValue(int(quadArray[1])))
        # Move to next instruction
        self.codeLine += 1

    def useDirFuncToAllocateInMethod(self, funcName, method):
        "Uses function name to allocate in specific memory allocation method from Memory object"
        # allocateLocalMemory(intCount, floatCount, charCount, stringCount)
        method(
            self.dirFunc[funcName]["int"],
            self.dirFunc[funcName]["float"],
            self.dirFunc[funcName]["char"],
            self.dirFunc[funcName]["string"]
        )

    def useDirFuncToAllocateTmpInMethod(self, funcName, method):
        "Uses function name to allocate in specific temporal memory allocation method from Memory object"
        method(
            self.dirFunc[funcName]["intTmp"],
            self.dirFunc[funcName]["floatTmp"],
            self.dirFunc[funcName]["charTmp"],
            self.dirFunc[funcName]["stringTmp"]
        )

    def era(self, quadArray):
        "Not used"
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
        "Instruction method placeholder"
        # Move to next instruction
        self.codeLine += 1

    def goto(self, quadArray):
        "Moves to code line"
        self.codeLine = int(quadArray[1])

    def gotoFalse(self, quadArray):
        "Moves to code line if false"
        # Evaluates if address value is False
        if self.toBool(self.memory.getValue(int(quadArray[1]))) == False:
            # Move to false instruction
            self.codeLine = int(quadArray[2])
        else:
            # Move to next instruction
            self.codeLine += 1

    def gotoTrue(self, quadArray):
        "Moves to code line if true"
        # Evaluates if address value is True
        if self.toBool(self.memory.getValue(int(quadArray[1]))) == True:
            # Move to true instruction
            self.codeLine = int(quadArray[2])
        else:
            # Move to next instruction
            self.codeLine += 1

    def gosub(self, quadArray):
        "Allocates function memory and puts needed parameters into new MemoryContext depending on dirFunc parameter type list"

        if '.' in quadArray[1]:
            self.goMethod(quadArray)
            return

        # Allocate local memory for dirFunc
        self.useDirFuncToAllocateInMethod(quadArray[1], self.memory.allocateLocalMemory)

        # Allocate local temp memory for dirFunc
        self.useDirFuncToAllocateTmpInMethod(quadArray[1], self.memory.allocateLocalTempMemory)

        # Get Param types from dirFunc
        paramTypes = self.dirFunc[quadArray[1]]["params"]
        paramValues = []

        # Pop params values from param stack depending on dirFunc param count
        for _ in paramTypes:
            paramValues.append(self.paramStack.pop())
        
        # Reverse params from stack to queue
        paramValues.reverse()

        # Ordered vars from function are assigned to local memory
        intCount = 0
        floatCount = 0
        charCount = 0
        stringCount = 0
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
            elif paramTypes[i] == "string":
                self.memory.setValue(self.memory.localString + stringCount, paramValues[i])
                stringCount += 1
            
        # Save function name and next instruction pointer to continue program after endfunc
        self.jumpStack.append((quadArray[1], self.codeLine + 1))

        # Moves to function code index
        self.codeLine = self.dirFunc[quadArray[1]]['index']

    def goMethod(self, quadArray):
        funcName = quadArray[1]
        className = quadArray[1].split('.')[0]

        objectParams = []

        values = {}

        totalToAssign = self.dirFunc[funcName].copy()
        varTypes = ["int", "float", "char", "string"]
        for t in varTypes:
            totalToAssign[t] += self.classes[className][t]
            
        varTypeCountdown = totalToAssign.copy()
        if len(self.objectParamStack) > 0:
            tmpObjParam = self.objectParamStack[-1]
            tmpObjParamType = self.memory.getType(tmpObjParam)
            # For each type
            # Pop the amount of needed variables for the object from the object param stack. 
            while varTypeCountdown[tmpObjParamType] > 0:
                self.objectParamStack.pop()
                varTypeCountdown[tmpObjParamType] -= 1
                objectParams.append((tmpObjParamType, tmpObjParam))
                if len(self.objectParamStack) == 0: break
                tmpObjParam = self.objectParamStack[-1]
                tmpObjParamType = self.memory.getType(tmpObjParam)
                
            # Reverse object params to make it in order
            objectParams.reverse()

        for t in varTypes:
            # Assign values in order for each var type
            values[t] = []
            for i in range(self.classes[className][t]):
                values[t].append(self.memory.getValue(objectParams[i][1]))
            

        self.memory.allocateLocalMemory(totalToAssign["int"], totalToAssign["float"], totalToAssign["char"], totalToAssign["string"])
        self.memory.allocateLocalTempMemory(totalToAssign["intTmp"], totalToAssign["floatTmp"], totalToAssign["charTmp"], totalToAssign["stringTmp"])

        # Get Param types from dirFunc
        paramTypes = self.dirFunc[quadArray[1]]["params"]
        paramValues = []

        # Pop params values from param stack depending on dirFunc param count
        for _ in paramTypes:
            paramValues.append(self.paramStack.pop())
        
        # Reverse params from stack to queue
        paramValues.reverse()

        # Ordered vars from function are assigned to local memory
        intCount = 0
        floatCount = 0
        charCount = 0
        stringCount = 0

        # Add object params to method call
        for i in range(len(objectParams)):
            if objectParams[i][0] == "int":
                if values["int"][intCount] is not None:
                    self.memory.setValue(self.memory.localInt + intCount, values["int"][intCount])
                intCount += 1
            elif objectParams[i][0] == "float":
                if values["float"][floatCount] is not None:
                    self.memory.setValue(self.memory.localFloat + floatCount, values["float"][floatCount])
                floatCount += 1
            elif objectParams[i][0] == "char":
                if values["char"][charCount] is not None:
                    self.memory.setValue(self.memory.localChar + charCount, values["char"][charCount])
                charCount += 1
            elif objectParams[i][0] == "string":
                if values["string"][stringCount] is not None:
                    self.memory.setValue(self.memory.localString + stringCount, values["string"][stringCount])
                stringCount += 1

        # Add function params to method call
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
            elif paramTypes[i] == "string":
                self.memory.setValue(self.memory.localString + stringCount, paramValues[i])
                stringCount += 1

        self.objectReturnDir.append(objectParams)

        # Save function name and next instruction pointer to continue program after endfunc
        self.jumpStack.append((quadArray[1], self.codeLine + 1))

        # Moves to function code index
        self.codeLine = self.dirFunc[quadArray[1]]['index']


    def assign(self, quadArray):
        "Assigns dir value to dir value"

        # Runs setValue method from Memory using flag usePointerOut=True 
        # which will make pointer assignments are solved before assignment
        self.memory.setValue(
            int(quadArray[2]),
            self.memory.getValue(int(quadArray[1])),
            usePointerOut=True
        )
        
        # Move to next instruction
        self.codeLine += 1

    def _print(self, quadArray):
        "Prints dir value"
        tmpVal = self.memory.getValue(int(quadArray[1]))
        if tmpVal == "\\n":
            print("")
        else:
            print(tmpVal, end='')
        
        # Move to next instruction
        self.codeLine += 1

    def strOperations(self, op, left, right):
        " + (int, 1) (str, 'text') 12345"
        " * (str, 'text') (int, 1) 12345"
        result = ""
        if op == '+':
            # concat
            result = str(left[1]) + str(right[1])
        elif op == '*':
            # generate and concat N of same string
            times = 0
            _str = ""
            if left[0] == int:
                times = left[1]
                _str = right[1]
            else:
                times = right[1]
                _str = left[1]
            
            for _ in range(times):
                result += _str
            

        return result
        


    def simpleOperator(self, quadArray):
        "Resolves quad depending on a simple Operator method of the form <operator leftOperand rightOperand outputDirectory>"

        # Operator
        op = quadArray[0]

        # Left Operand Dir
        lDir = int(quadArray[1])

        # Right Operand Dir
        rDir = int(quadArray[2])

        # Left Value
        lVal = self.memory.getValue(lDir, usePointerOut=True)
        lValType = type(lVal)

        # Right Value
        rVal = self.memory.getValue(rDir, usePointerOut=True)
        rValType = type(rVal)

        # Directory Out
        dirOut = int(quadArray[3])

        # Log operation
        self.consoleLog("{0} = {1} {2} {3}\n{0} = {4} {2} {5}".format(dirOut, lDir, op, rDir, lVal, rVal))

        outputValue = None

        # Run special string operations between types
        if (lValType != rValType) and (lValType == str or rValType == str):
            outputValue = self.strOperations(op, (lValType, lVal), (rValType, rVal))
        else:
            outputValue = self.simpleOperators[op](lVal, rVal)

        # Runs setValue on outputDirectory, use flag usePointerOut=False to make value is not resolved as pointer
        self.memory.setValue(
            dirOut,
            outputValue,
            usePointerOut=False
        )

        # Move to next instruction
        self.codeLine += 1


if __name__ == "__main__":
    if len(sys.argv) < 2:
        print("Usage: python AGRO.py programName [-d]")
    else:

        # Program name without extension
        programName = sys.argv[1]

        # Debug flag
        debug = False

        # Execution extra parameters
        for i in range(2, len(sys.argv)):
            flag = sys.argv[i]
            if flag == "-d":
                debug = True

        # AGRO VM CodeProcessor
        agroVM = CodeProcessor(programName, debug)
        
        # Print AGRO splash art
        print(splashArt)

        # Run code
        agroVM.processCode()
