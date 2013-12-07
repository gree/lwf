
if false then -- debug mode functions. (significant performance overhead when enabled.)
	ipairs_old=ipairs
	function ipairs(t,f)
		if t==nil then dbg.console('ipairs error') end
		return ipairs_old(t,f)
	end
end
dbg={ _count=1, linecolor="solidred"}
util=util or {}
function assert(bVal)
   if not bVal then
	  -- when a log function is defined
      if fineLog~=nil then
		  debug.sethook() -- stop all kinds of debugger
		  fineLog("assert failed")
		  fineLog(dbg.callstackString(3))
		  fineLog(util.tostring(dbg.locals()))
      end
      print("assert failed: type cs or dbg.traceBack() or help for more information.")
--      dbg.callstack()
--      debug.debug()
      dbg.console() -- my debugger.
   end
   return bVal
end

--[[
print2=print
function print(...)

	local a={...}
	if a[1]==700000 then
		dbg.console() 
	end
	print2(...)
end
]]--


function dbg.lunaType(c)
	if type(c)~='userdata' then
		return nil
	end
	local mt= getmetatable(c)
	if mt==nil then return nil end
	return mt.luna_class
end
function dbg.listLunaClasses(line)
	local usrCnam= string.sub(line, 7)
	local out2=''
	local out=''
	local outp=''
	
	for k,v in pairs(__luna)do
		if type(v)=='table' then
			local cname=v.luna_class
			if cname then
				local _, className=string.rightTokenize(cname,'%.')
				local nn=string.sub(k, 1, -string.len(className)-1)
				local namspac=string.gsub(nn,'_', '.')
				if namspac=='.' then namspac='' end
				if usrCnam=='' then
					out=out..namspac.. className..', '
				else
					if namspac..className==usrCnam then
						local map={__add='+', __mul='*',__div='/', __unm='-', __sub='-'}
						local lastFn='funcName'
						for kk,vv in pairs(v) do
							if type(vv)=='function' then
								if string.sub(kk,1,13)=='_property_get' then
									outp=outp .. string.sub(kk,15)..', '
								elseif string.sub(kk,1,13)=='_property_set' then
									--outp=outp .. string.sub(kk,15)..','
								else
									if map[kk] then
										out2=out2..map[kk]..', '
									else
										out2=out2..kk..', '
									end
									lastFn=kk
								end
							end
						end
						out2=out2..'\n\n Tip! you can see the function parameter types by typing "'..usrCnam..'.'..lastFn..'()"!'
						out2=out2..'\n Known bug: property names can be incorrectly displayed. "'
					end
				end
			end
		end
	end
	if outp~='' then print('Properties:\n', outp) end
	print(out)
	if out2~='' then print('Member functions:\n', out2) end
end
local err = pcall(function()
	dbg.rl = require 'readline'  
end)
-- Unix readline support, if readline.so is available...
function dbg.readLine(cursor)
	if not dbg.rl then
		io.write(cursor)
		return io.read('*line')
	else
		local line= dbg.rl.readline(cursor)
		if dbg.rl.saveline then
			dbg.rl.saveline(line)
		elseif dbg.rl.add_history then
			dbg.rl.add_history(line)
		end
		return line
	end
end

function dbg.traceBack(level)
   if level==nil then
      level=1
   end
   while true do
      local info=debug.getinfo(level)
      local k=info.name
      if k==nil then
	 break
      else
	 print('----------------------------------------------------------')
	 print('Level: ', level)
	 print(info.short_src..":"..info.currentline..":"..k)
	 print('Local variables:')
	 dbg.locals(level)
	 level=level+1
      end
   end
end
function os.VI_path()
	if os.isUnix() then
--		return "vim"  -- use vim in a gnome-terminal
		return "gvim"
	else
		return "gvim"
	end
end	

function os.toWindowsFileName(file)
	local fn=string.gsub(file, "/","\\")
	if string.sub(fn,1,1)=="\\" then
		fn=string.sub(fn,2,2)..':'..string.sub(fn,3)
	end
	return fn
end
function os.fromWindowsFileName(file)
	local fn=string.gsub(file, "\\","/")
	if string.sub(fn,2,2)==':' then
		fn='/'..string.sub(fn,1,1)..string.sub(fn,3)
	end
	return fn
end

function os.open(file)
	if os.isUnix() then
		os.execute('gnome-open "'..file..'"')
	else
		os.execute('start /b cmd /c "'..os.toWindowsFileName(file)..'"')
	end
end
function os.openFolder(folder)
	if os.isUnix() then
		os.execute('nautilus "'..folder..'"')
	else
		os.execute('explorer "'..os.toWindowsFileName(folder)..'"')
	end
end
function os.openTerminal(folder)
	if os.isUnix() then
		os.execute('gnome-terminal --working-directory "'..os.relativeToAbsolutePath(folder)..'"')
	else
		os.execute('cmd /k cd "'..os.toWindowsFileName(folder)..'"')
	end
end

function os.vi_check(fn)
	local otherVim='vim'
	local servers=string.tokenize(os.capture(otherVim..' --serverlist 2>&1',true), "\n")
	local out=array.filter(function (x) return string.upper(fn)==x end,servers)
	local out_error=array.filter(function (x) return string.find(x,"Unknown option argument")~=nil end,servers)
	if #out_error>=1 then return nil end
	return #out>=1
end

function os.vi_console_close_all()
	local servers=string.tokenize(os.capture('vim --serverlist',true), "\n")
	local out=array.filter(function (x) return fn~="GVIM" end,servers)
	for i,v in ipairs(out) do
		os.execute('vim --servername "'..v..'" --remote-send ":q<CR>"')
	end
end

function os.vi_console_cmd(fn, line)
	local cc
	if line then
		cc=' +'..line..' "'..fn..'"'
	else
		cc=' "'..fn..'"'
	end
	if not os.isUnix() and os.isFileExist("C:/msysgit/msysgit/share/vim/vim73/vim.exe") then
		return '"C:/msysgit/msysgit/share/vim/vim73/vim.exe" '..cc
	end
	return 'vim '..cc
end
function os.gedit_cmd(fn, line)
	local cc
	if line then
		cc=' +'..line..' "'..fn..'"'
	else
		cc=' "'..fn..'"'
	end
	return 'gedit '..cc
end
function os.vi_readonly_console_cmd(fn, line)
	local cc
	if line then
		cc=' +'..line..' "'..fn..'"'
	else
		cc=' "'..fn..'"'
	end

	return 'vim -R -M -c ":set nomodifiable" '..cc
end
function os.vi_line(fn, line)
	if os.vi_check(fn) then
		os.execute2(os.vi_console_cmd(fn,line))
		return
	end
	if not os.launch_vi_server() then
		print('Please launch gvim first!')
		return
	end
	local VI=os.VI_path()..' --remote-silent'
	local cmd=VI..' +'..line..' "'..fn..'"'
	--print(cmd)
	os.execute2(cmd)
end
function os.launch_vi_server()
	local lenvipath=string.len(os.VI_path())
	if os.vi_check(string.upper(os.VI_path())) then
		print("VI server GVIM open")
		return true
	end
	if false then -- recent ubuntu gvim doesn't start up from a terminal.
		print("launching GVIM server...")
		if os.isUnix() then
			if os.VI_path()=="vim" then
				os.execute2('cd ../..', 'gnome-terminal -e "vim --servername vim"&') -- this line is unused by default. (assumed gnome dependency)
			else
				os.execute2('cd ../..', os.VI_path())
			end
		else
			if os.isFileExist(os.capture('echo %WINDIR%').."\\vim.bat") then
				os.execute2('cd ..\\..', os.VI_path())
			else
				os.execute2('cd ..\\..', "start "..os.VI_path())
			end
		end

		for i=1,10 do
			if os.vi_check(string.upper(os.VI_path())) then
				print("VI server GVIM open")
				break
			else
				print('.')
				--os.sleep(1)
			end
		end
		return true
	end
	return false
end
function os.vi(...)
	os._vi(os.VI_path(), ...)
end

function os._vi(servername, ...)
	local VI=os.VI_path() ..' --servername '..servername..' --remote-silent'
	local VI2=os.VI_path() ..' --servername '..servername..' --remote-send ":n '
	local VI3='<CR>"'

	local targets={...}

	local otherVim='vim'

	local vicwd=os.capture(otherVim..' --servername '..servername..' --remote-expr "getcwd()"')
	if vicwd=="" then
		if not os.launch_vi_server() then
			print('Please launch gvim first!')
			return
		end
		-- try one more time
		vicwd=os.capture(otherVim ..' --servername '..servername..' --remote-expr "getcwd()"')
	end
	print('vicwd=',vicwd)
	local itgt, target
	for itgt,target2 in ipairs(targets) do
		--      local target=string.sub(target2,4)
		local target=target2

		if string.find(target, '*') ~=nil or string.find(target, '?')~=nil then

			if false then
				-- open each file. too slow
				local subtgts=os.glob(target)

				local istgt,subtgt
				for istgt,subtgt in ipairs(subtgts) do
					local cmd=VI..' "'..subtgt..'"'
					if string.find(cmd,'~')==nil then
						os.execute(cmd) 
					end
				end
			elseif string.sub(target, 1,6)=="../../" and string.sub(vicwd, -10)=="taesoo_cmu" then -- fastest method 
				local cmd=VI2..string.sub(target,7)..VI3
				print(cmd)
				if os.isUnix() then
					os.execute(cmd.."&")
				else
					os.execute("start "..cmd)
				end
			else
				local lastSep
				local newSep=0
				local count=0
				repeat lastSep=newSep
					newSep=string.find(target, "/", lastSep+1) 	    
					count=count+1
				until newSep==nil 

				local path=string.sub(target, 0, lastSep-1)

				local filename
				if lastSep==0 then filename=string.sub(target,lastSep) else filename=string.sub(target, lastSep+1) end

				print(filename, path, count)

				print("cd "..path, VI.." "..filename)
				if os.isUnix() then
					os.execute2("cd "..path, "rm -f *.lua~", VI.." "..filename.."&")
				else
					os.execute2("cd "..path, "rm -f *.lua~", "rm -f #*#", VI.." "..filename)
				end
				--	    end

			end

		else
			local cmd=VI..' "'..target..'"'
			print(cmd)
			if os.isUnix() then
				os.execute(cmd.."&")
			else
				os.execute(cmd)
			end
		end
	end
 end


function dbg.showCode(fn,ln)
	util.iterateFile(fn,
	{ 
		iterate=function (self, lineno, c) 
			if lineno>ln-5 and lineno<ln+5 then
				c=string.gsub(c, "\t", "    ")
				if #c > 70 then
					c=string.sub(c,1,65).."..."
				end
				if lineno==ln then
					print(lineno.."* "..c)
				else
					print(lineno.."  "..c)
				end
			end
		end
	}
	)
end

-- e.g. dbg.setFunctionHook(RE, 'createVRMLskin')
function dbg.setFunctionHook(table, functionName)
	dbg[functionName..'_old']=table[functionName]
	table[functionName]=function (...)
		print('Function :'..functionName)
		dbg.console()
		return dbg[functionName..'_old'](...)
	end
end
function dbg.console(msg, stackoffset)

	stackoffset=stackoffset or 0
	if(msg) then print (msg) end
	
	if dbg._consoleLevel==nil then
		dbg._consoleLevel=0
	else
		dbg._consoleLevel=dbg._consoleLevel+1
	end
      if coarseLog~=nil and rank~=nil then
		  debug.sethook() -- stop all kinds of debugger
		  coarseLog("dbg.console called")
		  coarseLog(dbg.callstackString(1))
		  coarseLog(util.tostring(dbg.locals()))
		  dbg.callstack0()
		  return
      end
	local function at(line, index)
		return string.sub(line, index, index)
	end

	local function handleStatement(statement)

		local output
		if string.find(statement, "=") and not string.find(statement, "==") then -- assignment statement
			output={pcall(loadstring(statement))}
		else -- function calls or print variables: get results
			output={pcall(loadstring("return ("..statement..")"))}
			if output[1]==false and output[2]=="attempt to call a nil value" then
				-- statement
				output={pcall(loadstring(statement))}
			end
		end

		if output[1]==false then 
			print("Error! ", output[2]) 
		else
			if type(output[1])~='boolean' then
				output[2]=output[1] -- sometimes error code is not returned for unknown reasons.
			end
			if type(output[2])=='table' then 
				if getmetatable(output[2]) and getmetatable(output[2]).__tostring then
					print(output[2])
				else
					printTable(output[2]) 
				end
			elseif output[2] then
				dbg.print(unpack(table.isubset(output, 2)))
			elseif type(output[2])=='boolean' then
				print('false')
			end
		end
	end

	local event
	while true do
		local cursor="[DEBUG"..dbg._consoleLevel.."] > "
		line=dbg.readLine(cursor)
		local cmd=at(line,1)
		local cmd_arg=tonumber(string.sub(line,2))
		if not (string.sub(line,2)=="" or cmd_arg) then
			if not ( cmd=="r" and at(line,2)==" ") then
				if not string.isOneOf(cmd, ":", ";") then
					cmd=nil
				end
			end
		end

		if cmd=="h" or string.sub(line,1,4)=="help" then --help
			print('bt[level=3]      : backtrace. Prints callstack')
			print(';(lua statement) : eval lua statements. Usually, ";" can be omitted. e.g.) print(a) ')
            print('                   print or printTable can be omitted too            e.g.) a ')
			print(':(lua statement) : eval lua statements and exit debug console. e.g.) :dbg.startCount(10)')
			print('s[number=1]      : proceed n steps')
            print('fi               : finish the current function')
			print('r filename [lineno]  : run until execution of a line. filename can be a postfix substring. e.g.) r syn.lua 32')
			print('c[level=2]       : print source code at a stack level')
			print('e[level=2]       : show current line (at callstack level 2) in gedit editor')
			print('v[level=2]       : show current line (at callstack level 2) in vi editor')
			print('c[level=2]       : show nearby lines (at callstack level 2) here')
			print('l[level=2]       : print local variables. Results are saved into \'l variable.')
			print("                      e.g) DEBUG]>print('l.self.mVec)")
			print('clist            : list luna classes')
			print('clist className  : list functions in the class')
			print('cont             : exit debug mode')
			print('global variables : Simply type "a" to print the content of a global variable "a".')
			print('local variables  : Simply type "`a" to print the content of a local variable "a".')
			print('lua statement    : run it')
		elseif line=="cont" then break
		elseif string.sub(line,1,2)=="bt" then dbg.callstack(tonumber(string.sub(line,3)) or 3)
		elseif line=="clist" or string.sub(line,1,6)=='clist ' then
			dbg.listLunaClasses(line)		
		elseif cmd=="c" or cmd=="v" then
			if cmd_arg==nil then
				local level=stackoffset
				while true do
					local info=debug.getinfo(level)
					if info then
						local a=string.sub(info.source, 1,1)
						if a=='=' or a=='[' then
							level=level+1
						elseif select(1,string.find(info.source, 'mylib.lua')) then
							level=level+1
						else
							break
						end
					else
						level=level+1
						if level>40 then break end
					end
				end
				cmd_arg=level-stackoffset+1
				print('c'..cmd_arg..':')
			end
			local level=(cmd_arg or 1)+stackoffset-1 -- -1 means 'excluding dbg.showCode'
			local info=debug.getinfo(level)
			if info then
				local a=string.sub(info.source, 1,1)
				if a=='=' or a=='[' then
					print(info.source)
				else
					local ln=info.currentline
					print(string.sub(info.source,2))
					if cmd=="v" then
						local fn=string.sub(info.source,2)
						fn=os.relativeToAbsolutePath(fn)
						os.vi_line(fn,info.currentline)
					else
						dbg.showCode(string.sub(info.source,2),ln)
						dbg._saveLocals=dbg.locals(level+1,true)
					end
				end
			else
				print('no such level')
			end
		elseif cmd=="e" then
			local info=debug.getinfo((cmd_arg or 1)+stackoffset-1)
			if info then
			   os.execute(os.gedit_cmd(os.relativeToAbsolutePath(string.sub(info.source,2)),info.currentline)..'&')
			end
		elseif cmd==";" then
			handleStatement(string.sub(line,2))
		elseif cmd==":" then
			handleStatement(string.sub(line,2))
			break
		elseif cmd=="s" or cmd=="'" then
			local count=cmd_arg or 1
			event={"s", count}
			break
		elseif cmd=="r" then
			event={"r", string.sub(line, 3)}
			break
		elseif line=="f" or line=="fi" or line=="fin" or line=="fini" or line=="finish" then
			event={"fi"}
			break
		elseif cmd=="l" then
			local level=(cmd_arg or 1)
			dbg._saveLocals=dbg.locals(level)
		else 
			statement=string.gsub(line, '``', 'dbg._saveLocals')
			statement=string.gsub(line, '`', 'dbg._saveLocals.')
			handleStatement(statement)
		end
	end

	dbg._consoleLevel=dbg._consoleLevel-1
	if event then
		if event[1]=="s" then 
			return dbg.step(event[2]) 
		elseif event[1]=="r" then
			return dbg.run(event[2])
		elseif event[1]=="fi" then
			return dbg.finish(event[2])
		end
	end

end

function dbg._stepFunc (event, line)
   dbg._step=dbg._step+1
   if dbg._step==dbg._nstep then
      debug.sethook()
	  local level=2
	  local info=debug.getinfo(level)
	  if info then
		  if select(1,string.find (info.source, 'mylib.lua')) then
			  return dbg.step(1)
		  end
		  print(info.source, info.currentline)
		  dbg.showCode(string.sub(info.source,2), info.currentline)
		  dbg._saveLocals=dbg.locals(level+1,true)
	  end
      return dbg.console()
   end
end
function dbg.step(n) 
   dbg._step=0
   dbg._nstep=n
   debug.sethook(dbg._stepFunc, "l")	
end
function dbg.callstack(level)
	if level==nil then
		level=1
	end
	while true do
		local info=debug.getinfo(level)
		local k=info.name
		if k==nil then
			break
		else
			print(info.short_src..":"..info.currentline..":"..k)
			level=level+1
		end
	end
end

function dbg._finishFunc(event, line)
	for i=1,16 do
		-- search the current function from stack 1 to 16
		local info=debug.getinfo(i)
		if info then
			if info.func==dbg._finishFunc_until then
				return
			end
		else
			break
		end
	end
	debug.sethook()
	local level=2
	local info=debug.getinfo(level)
	if info then
		if select(1,string.find (info.source, 'mylib.lua')) then
			return dbg.step(1)
		end
		print(info.source, info.currentline)
		dbg.showCode(string.sub(info.source,2), info.currentline)
		dbg._saveLocals=dbg.locals(level+1,true)
	end
	return dbg.console()
end

function dbg.finish(n)
	local info=debug.getinfo(3)
	if info then
		if info.source=="=(tail call)" then
			info=debug.getinfo(4)
		end
		print('run until ', info.name, 'finishes :', info.source, info.func)
		dbg._finishFunc_until=info.func
		debug.sethook(dbg._finishFunc, "l")	
	else
		print('cannot find the current function')
	end
end

function dbg.callstack0(level)
	if level==nil then
		level=1
	end
	while true do
		local info=debug.getinfo(level)
		if info==nil then break end
		local k=info.name
		if k==nil then
			printTable(info)
			level=level+1
		else
			print(info.short_src..":"..info.currentline..":"..k)
			level=level+1
		end
	end
end
function dbg.locals(level, noprint)
	local output={}
	if level==nil then level=1 end
	cur=1
	while true do
		if debug.getinfo(level, 'n')==nil then return output end
		k,v=debug.getlocal(level, cur)
		if k~=nil then
			output[k]=v or "(nil)"
			cur=cur+1
		else
			break
		end
	end
	if not noprint then
		os.print(output)
	end
	return output
end
function dbg.run(run_str) -- run_str example: a.lua 374 
   local tbl=string.tokenize(run_str, " ")
   local filename=tbl[1]
   local lineno=tonumber(tbl[2])

   --print(filename..","..tostring(lineno))
   if tonumber(filename)~=nil then
	   lineno=tonumber(filename)
	   filename=''
   end
   if filename=='' then
	   local info=debug.getinfo(3)
	   filename=info.source
	   if string.sub(info.source,1,1)=="=" then
		   info=debug.getinfo(4)
		   filename=info.source
	   end
   end
   print("stop at "..filename.." +",lineno)
   local strlen=string.len(filename)*-1
   dbg._runFuncParam={filename, strlen, lineno}
   debug.sethook(dbg._runFunc, "l")	
end

function dbg._runFunc (event, line)
   local src=debug.getinfo(2).source
   local param=dbg._runFuncParam

   if string.sub(src, param[2])==param[1] and (param[3]==nil or line==param[3]) then
      debug.sethook()
      print(debug.getinfo(2).source, line)
      return dbg.console()
   end
end
-- outputs counts to trace.txt
function dbg.startCount(dbgtime)
	if dbg.filePtr==nil then
		if dbgtime then
			print('Start re-counting until '..dbgtime)
		else
			print('Start counting.. ')
			print('Output will go to trace.txt')
			print('You can debug a crashing program by re-running the program using dbg.startCount(lastCount)')
			dbg.filePtr, msg=io.open("trace.txt", "w")
			if dbg.filePtr==nil then
				print(msg)
				return
			end
		end

		dbg._dbgtime=dbgtime
		dbg._count=0
		--debug.sethook(dbg.countHookF, "l")
		debug.sethook(dbg.countHookF, "c") -- much faster though less accurate
	else
	end
end
function dbg.countHookF(event)
	local _count=dbg._count 
	local _dbgtime=dbg._dbgtime

	if _dbgtime then
		if _count>_dbgtime-100 then
			local info=debug.getinfo(2)
			print('coundown', _dbgtime-_count, info.name, info.short_src, info.currentline)
			if _count==_dbgtime then
				debug.sethook()
				dbg.console()
			end
		end
	else
		local filePtr=dbg.filePtr
		filePtr:seek("set", 0)
		filePtr:write(_count)
		filePtr:flush()
	end
	dbg._count=_count+1
end

-- collection of utility functions that depends only on standard LUA. (no dependency on baseLib or mainLib)
-- all functions are platform independent

function string.trimSpaces(s)
  s = string.gsub(s, '^%s+', '') --trim left spaces
  s = string.gsub(s, '%s+$', '') --trim right spaces
  return s
  end
  function string.startsWith(a,b)
	  return string.sub(a,1,string.len(b))==b
  end
function os.capture(cmd, raw)
  local f = assert(io.popen(cmd, 'r'))
  local s = assert(f:read('*a'))
  f:close()
  if raw then return s end
  s = string.gsub(string.trimSpaces(s), '[\n\r]+', ' ') 
  return s
end
function os.rightTokenize(str, sep, includeSep)
	--deprecated
	return string.rightTokenize(str, sep, includeSep)
end

function string.rightTokenize(str, sep, includeSep)
	local len=string.len(str)
	for i=len,1,-1 do
		local s=string.find(string.sub(str, i,i),sep)
		if s then
			if includeSep then
				return string.sub(str, 1, i-1)..sep, string.sub(str, i+1)
			end
			return string.sub(str, 1, i-1), string.sub(str, i+1)
		end
	end
	return "", str
end


function os.sleep(aa)
   local a=os.clock()
   while os.difftime(os.clock(),a)<aa do -- actually busy waits rather then sleeps
   end
end

function string.isLongestMatched(str, patterns, prefix, postfix)
	local matched=nil
	local ll=0
	for k,ip in ipairs(patterns) do
		if str==ip then return k end -- exact match has the highest priority
		if prefix then ip=prefix..ip end
		if postfix then ip=ip..postfix end
		local idx,idx2=string.find(str, ip)
		
		if idx~=nil then
			if idx2-idx>=ll then
				matched=k
				ll=idx2-idx
			end
		end
	end
	return matched
end

function string.isMatched(str, patterns)
	local matched=nil
	for k,ip in ipairs(patterns) do
		local idx=string.find(str, ip)
		if idx~=nil or str==ip then
			matched=k
		end
	end
	return matched
end
function string.findLastOf(str, pattern)
	local lastS=nil
	local idx=0
	while idx+1<#str do
		idx=string.find(str, pattern, idx+1)
		if idx then
			lastS=idx
		else
			break
		end
	end
	return lastS
end
function os.isUnix()  -- has posix commands
	local isWin=string.find(string.lower(os.getenv('OS') or 'nil'),'windows')~=nil
	if isWin then
		if string.find(string.lower(os.getenv('PATH') or 'nil'), ':/cygdrive') then
			return true
		end
		return false
	end
	return true
end

function os.isMsysgit()
	local isMsysgit=string.find(string.lower(os.getenv('PATH') or 'nil'), 'msysgit')~=nil
	return isMsysgit
end
function os.isWindows()
	local isWin=string.find(string.lower(os.getenv('OS') or 'nil'),'windows')~=nil
	return isWin
end


-- LUAclass method is for avoiding so many bugs in luabind's "class" method (especially garbage collection).

-- usage: MotionLoader=LUAclass()
--   ...  MotionLoader:__init(a,b,c)

--        VRMLloader=LUAclass(MotionLoader)
--   ...  VRMLloader:__init(a,b,c)
--              MotionLoader.__init(self,a,b,c)
--        end

--  loader=VRMLloader(a,b,c) 

function LUAclass(baseClass)

	local classobj={}
	classobj.__index=classobj
	classobj.new=function (classobj, ...)
		local new_inst={}
		setmetatable(new_inst, classobj)
		new_inst:__init(...)
		return new_inst
	end

	if baseClass~=nil then
		if baseClass.luna_class then
			local derivedName=baseClass.luna_class..'_derived'
			while __luna['_'..derivedName] do
				derivedName=derivedName..'_'
			end
			__luna['_'..derivedName]=classobj
			classobj.luna_class=derivedName
			if baseClass.new_modified_T==nil then
				print("Error!".. baseClass.luna_class.." doesn't have new_modified_T member!")
				print("Did you forget to set isLuaInheritable=true?")
			end
			for k,v in pairs(baseClass) do
				classobj[k]=v
			end

			classobj.new=function (classobj, ...)
							 local new_inst=classobj.new_modified_T(classobj, '_'..derivedName) -- has to have default constructor. 
							 new_inst:__init(...)
							 return new_inst
						 end
		end
		setmetatable(classobj, {__index=baseClass,__call=classobj.new})
	else
		setmetatable(classobj, {__call=classobj.new})
	end
	return classobj
end

function LUAclass_getProperty(t, nocheck)
	local result={}
	if type(t)=="userdata" then
		result=t:toTable() -- {"__userdata", typeName, type_specific_information...}
	elseif type(t)=='table' and getmetatable(t) and t.toTable and not nocheck then
		result=t:toTable()
	elseif type(t)=="table" then
		for k, v in pairs(t) do
			if k=="__index" then
				-- do nothing
			elseif k=="__newindex" then
				-- do nothing
			elseif type(v)=="function" then
				-- do nothing
			elseif type(v)=="table" or type(v)=="userdata" then
				result[k]=LUAclass_getProperty(v)
			else
				result[k]=v
			end
		end
	else
		result=t
	end
	return result
end
function LUAclass_setProperty(result, t)
	for k, v in pairs(t) do
		if type(v)=='table' and v[1]=="__userdata" then
			result[k]=_G[v[2]].fromTable(v)
		elseif type(v)=='table' then
			assert(result[k] and type(result[k])=='table')
			LUAclass_setProperty(result[k], v)
		else
			result[k]=v
		end
	end
end
-- one indexing. 100% compatible with original lua table. (an instance is an empty table having a metatable.)
array=LUAclass()

--[[
    zip_with_helper ()

    This is a generalized version of Haskells zipWith, but instead
    of running a function and appending that result to the list of results
    returned, we call a helper function instead.

    So this function does most of the work for map(), filter(), and zip().

    result_helper may do a variety of things with the function to
    be called and the arguments.  The results, if any, are appended
    to the resutls_l table.
]]--
local function zip_with_helper(result_helper, rh_arg, ...)
     local results_l= {}
     local args = {...}     -- a table of the argument lists
     local args_pos = 1     -- position on each of the individual argument lists
     local have_args = true

     while have_args do
        local arg_list = {}
        for i, v in ipairs(args) do
            local a = nil
            a = v[args_pos]
            if a then
                arg_list[i] = a
            else
                have_args = false
                break
            end
        end
        if have_args then
            result_helper(rh_arg, arg_list, results_l)
        end
        args_pos = args_pos + 1
    end
                    
     return results_l
end

 --[[
    filter(func, [one or more tables])

    Selects the items from the argument list(s), calls
    func() on that, and if the result is true, the arguments
    are appended to the results list.

    Note that if func() takes only one argument and one
    list of arguments is given, the result will be a table
    that contains the values from the argument list directly.

    If there are two or more argument lists, then the 
    result table will contain a list of lists of arguments that matched
    the condition implemented by func().

    Examples:
        function is_equal (x, y) return x == y end
        function is_even (x) return x % 2 == 0 end
        function is_less (x, y) return x < y end


        filter(is_even, {1,2,3,4}) -> {2,4}

        filter(is_equal, {10, 22, 30, 44, 40}, {10, 20, 30, 40})   --> {{10,10}, {30, 30}}

        filter(is_less, {10, 20, 30, 40}, {10, 22, 33, 40})        --> {{20,22}, {30, 33}}

 ]]--
local function filter_helper (func, arg_list, results_l)
    local result = func(unpack(arg_list))
    if result then
        if #arg_list == 1 then
            table.insert(results_l, arg_list[1])
        else
            table.insert(results_l, arg_list)
        end
    end
end
function array.sub(arr,first,last)
	return table.isubset(arr,first,last)
end


function array.filter(func, ...)
    return zip_with_helper(filter_helper, func, ...)
end


 --[[
    map(function, [one or more tables])

    Repeatedly apply the function to the arguments composed from the
    elements of the lists provided.

    Examples:
        function double(x) return x * 2 end
        function add(x,y) return x + y end

        map(double, {1,2,3})                -> {2,4,6}
        map(add, {1,2,3}, {10, 20, 30})     -> {11, 22, 33}

    This also implements the functionality of 
        zipWith, zipWith3, zipWith4, etc. 
    in Haskell.

    func() should be a function that takes as many
    arguments as tables provided.  map() returns a list of just the
    first return values from each call to func().
 ]]--
local function map_helper (func, arg_list, results_l)
    table.insert(results_l, func(unpack(arg_list)))
end

function array.map(func, ...)
    return zip_with_helper(map_helper, func, ...)
end
 --[[
    foldr() - list fold right, with initial value

    foldr(function, default_value, table)

    Example:
        function mul(x, y) return x * y end
        function div(x, y) return x / y end

        foldr(mul, 1, {1,2,3,4,5})  ->  120
        foldr(div, 2, {35, 15, 6})  ->  7
 ]]--
function array.foldr(func, val, tbl)
    for i = #tbl, 1, -1 do
        val = func(tbl[i], val)
    end
    return val
end
function array:__init()
end
function array:clear()
	self={}
	setmetatable(self,array)
end

function array:size()
	return table.getn(self)
end

function array:pushBack(...)
	assert(self)
	for i, x in ipairs({...}) do
		table.insert(self, x)
	end
end

function array:popFront()
	local out=self[1]
	for i=1,#self-1 do
		self[i]=self[i+1]
	end
	self[#self]=nil
	return out
end

function array:pushBackIfNotExist(a)
	for i, v in ipairs(self) do
		if self[i]==a then
			return 
		end
	end
	array.pushBack(self, a)
end

function array:concat(tbl)
	for i, v in ipairs(tbl) do 
		table.insert(self, v)
	end
end

-- input : tables
function array.concatMulti(...)
	local out={}
	for i,v in ipairs({...}) do
		assert(type(v)=='table')
		array.concat(out,v)
	end
	return  out
end


function array:removeAt(i)
	table.remove(self, i)
end


function array:assign(tbl)
   for i=1,table.getn(tbl) do
      self[i]=tbl[i]
   end
end
function array:remove(...)
   local candi={...}
   if type(candi[1])=='table' then
      candi=candi[1]
   end

   local backup=array:new()
   for i=1,table.getn(self) do
      backup[i]={self[i], true}
   end
   
   for i, v in ipairs(candi) do
      backup[v][2]=false
   end

   local count=1
   for i=1, table.getn(backup) do
      if backup[i][2] then
	 self[count]=backup[i][1]
	 count=count+1
      end
   end

   for i=count, table.getn(backup) do
      self[i]=nil
   end

end

function array:back()
	return self[table.getn(self)]
end

function string.join(tbl, sep)
	return table.concat(tbl, sep)
end

function string.isOneOf(str, ...)
	local tbl={...}
	for i,v in ipairs(tbl) do
		if str==v then
			return true
		end
	end
	return false
end

-- similar to string.sub
function table.isubset(tbl, first, last)

	if last==nil then last=table.getn(tbl) end
	if last<0 then last=table.getn(tbl)+last+1 end

	local out={}
	for i=first,last do
		out[i-first+1]=tbl[i]
	end
	return out
end

function table.find(tbl, x)
	for k, v in pairs(tbl) do
		if v==x then 
			return k
		end
	end
	return nil
end
function table.filter(fcn, tbl)
	local out={}
	for k,v in pairs(tbl) do
		if fcn(k,v) then
			out[k]=v
		end
	end
	return out
end
function table.filterByKey(pattern, tbl)
	local out={}
	for k,v in pairs(tbl) do
		if select(1,string.find(k,pattern) ) then
			out[k]=v
		end
	end
	return out
end

function table._ijoin(tbl1, tbl2)
	local out={}
	local n1=table.getn(tbl1)
	local n2=table.getn(tbl2)
	for i=1,n1 do
		out[i]=tbl1[i]
	end

	for i=1,n2 do
		out[i+n1]=tbl2[i]
	end
	return out
end

function table.ijoin(...)
	local input={...}
	local out={}
	for itbl, tbl in ipairs(input) do
		out=table._ijoin(out, tbl)
	end
	return out
end

function table.join(...)
	local input={...}
	local out={}
	for itbl, tbl in ipairs(input) do
		for k,v in pairs(tbl) do
			out[k]=v
		end
	end
	return out
end

function table.mult(tbl, b)

	local out={}
	for k,v in pairs(tbl) do
		out[k]=v*b
	end
	setmetatable(out,table)
	return out
end


function table.add(tbl1, tbl2)

	local out={}

	for k,v in pairs(tbl1) do
		if tbl2[k] then
			out[k]=tbl1[k]+tbl2[k]
		end
	end
	for k,v in pairs(tbl2) do
		if tbl1[k] then
			out[k]=tbl1[k]+tbl2[k]
		end
	end
	setmetatable(out,table)

	return out
end
table.__mul=table.mult
table.__add=table.add

function pairsByKeys (t, f)
   local a = {}
   for n in pairs(t) do table.insert(a, n) end
   if f==nil then
	   f=function (a,b) -- default key comparison function
		   if type(a)==type(b) then
			   return a<b
		   end
		   return type(a)<type(b)
	   end
   end
   table.sort(a, f)
   local i = 0      -- iterator variable
   local iter = function ()   -- iterator function
		   i = i + 1
		   if a[i] == nil then return nil
		   else return a[i], t[a[i]]
		   end
		end
   return iter
end

function printTable(t, bPrintUserData, maxLen)
	maxLen=maxLen or 80
	print('{')
	for k,v in pairsByKeys(t) do
		local tv=type(v)
		if tv=="string" or tv=="number" or tv=="boolean" then
			print('\t['..k..']='..tostring(v)..', ')
		elseif tv=="userdata" then
			if bPrintUserData==true then
				print('\t['..k..']=\n'..v..', ')
			else
				print('\t['..k..']=('..tv..'), ')
			end
		elseif tv=="table" then
			print('\t['..k..']='..table.toPrettyString(v, maxLen))
		else
			print('\t['..k..']='..tv..', ')
		end
	end
	print('}')
end

function table.grep(t, pattern)
	local tbl={}
	for k,v in pairs(t) do
		if select(1, string.find(k,pattern)) then
			tbl[k]=v
		end
	end
	return tbl
end


function table.fromstring(t_str)
	local fn=loadstring("return "..t_str)
	if fn then
		local succ,msg=pcall(fn)
		if succ then
			return msg
		else
			print('pcall failed! '..t_str..","..msg)
		end
	else
		print('compile error')
	end
	return nil
end

function table.tostring2(t)
	return table.tostring(util.convertToLuaNativeTable(t))
end
function table.fromstring2(t)
	return util.convertFromLuaNativeTable(table.fromstring(t))
end
function table.toHumanReadableString(t, spc)
	spc=spc or 4
	-- does not check reference. so infinite loop can occur.  to prevent
	-- such cases, use pickle() or util.saveTable() But compared to pickle,
	-- the output of table.tostring is much more human readable.  if the
	-- table contains userdata, use table.tostring2, fromstring2 though it's
	-- slower.  (it preprocess the input using
	-- util.convertToLuaNativeTable 
	-- a=table.tostring(util.convertToLuaNativeTable(t)) convert to
	-- string t=util.convertFromLuaNativeTable(table.fromstring(a)) 
	-- convert back from the string)

	local out="{"

	local N=table.getn(t)
	local function packValue(v)
		local tv=type(v)
		if tv=="number" or tv=="boolean" then
			return tostring(v)
		elseif tv=="string" then
			return '"'..tostring(v)..'"'
		elseif tv=="table" then
			return table.toHumanReadableString(v,spc+4)
		end
	end

	for i,v in ipairs(t) do
		out=out..packValue(v)..", "
	end

	for k,v in pairsByKeys(t) do

		local tk=type(k)
		local str_k
		if tk=="string" then
			str_k="['"..k.."']="
			out=out..string.rep(' ',spc)..str_k..packValue(v)..',\n '
		elseif tk~="number" or k>N then	 
			str_k='['..k..']='
			out=out..string.rep(' ',spc)..str_k..packValue(v)..',\n '
		end
	end
	return out..'}\n'
end
function table.tostring(t)
	-- does not check reference. so infinite loop can occur.  to prevent
	-- such cases, use pickle() or util.saveTable() But compared to pickle,
	-- the output of table.tostring is much more human readable.  if the
	-- table contains userdata, use table.tostring2, fromstring2 though it's
	-- slower.  (it preprocess the input using
	-- util.convertToLuaNativeTable 
	-- a=table.tostring(util.convertToLuaNativeTable(t)) convert to
	-- string t=util.convertFromLuaNativeTable(table.fromstring(a)) 
	-- convert back from the string)

	local out="{"

	local N=table.getn(t)
	local function packValue(v)
		local tv=type(v)
		if tv=="number" or tv=="boolean" then
			return tostring(v)
		elseif tv=="string" then
			return '"'..tostring(v)..'"'
		elseif tv=="table" then
			return table.tostring(v)
		else 
			return tostring(v)
		end
	end

	for i,v in ipairs(t) do
		out=out..packValue(v)..", "
	end

	for k,v in pairs(t) do

		local tk=type(k)
		local str_k
		if tk=="string" then
			str_k="['"..k.."']="
			out=out..str_k..packValue(v)..', '
		elseif tk~="number" or k>N then	 
			str_k='['..k..']='
			out=out..str_k..packValue(v)..', '
		end
	end
	return out..'}'
end

function table.toPrettyString(t, maxLen)
	if maxLen<0 then
		return ' ...'
	end
	local out="{"

	local N=table.getn(t)
	local function packValue(v, maxLen)
		local tv=type(v)
		if tv=="number" or tv=="boolean" then
			return tostring(v)
		elseif tv=="string" then
			return '"'..tostring(v)..'"'
		elseif tv=="table" then
			return table.toPrettyString(v,maxLen)
		else 
			return tostring(v)
		end
	end

	for i,v in ipairs(t) do
		out=out..packValue(v,maxLen-#out)..", "
		if #out>maxLen then
			return out ..'...}'
		end
	end

	for k,v in pairs(t) do

		local tk=type(k)
		local str_k
		if tk=="string" then
			str_k="['"..k.."']="
			out=out..str_k..packValue(v, maxLen-#out)..', '
		elseif tk~="number" or k>N then	 
			str_k='['..k..']='
			out=out..str_k..packValue(v, maxLen-#out)..', '
		end
		if #out>maxLen then
			return out ..'...}'
		end
	end
	return out..'}'
end
function table.remove_if(table, func)
	for k,v in pairs(table) do
		if func(k,v) then
			table[k]=nil
		end
	end
end

function util.chooseFirstNonNil(a,b,c)
	if a~=nil then return a end
	if b~=nil then return b end
	return c
end
function util.convertToLuaNativeTable(t)
	local result={}
	if type(t)=="userdata" then
		result=t:toTable() -- {"__userdata", typeName, type_specific_information...}
	elseif type(t)=='table' and getmetatable(t) and t.toTable then
		result=t:toTable()
	elseif type(t)=="table" then
		for k, v in pairs(t) do
			if type(v)=="table" or type(v)=="userdata" then
				result[k]=util.convertToLuaNativeTable(v)
			else
				result[k]=v
			end
		end
	else
		result=t
	end
	return result
end

function util.convertFromLuaNativeTable(t)
	local result
	if type(t)=="table" then
		if t[1]=="__userdata" then
			result=_G[t[2]].fromTable(t)
		else
			result={}
			for k, v in pairs(t) do
				result[k]=util.convertFromLuaNativeTable(v)
			end
		end
	else
		result=t
	end

	return result
end

function util.readFile(fn)
	if os.isWindows() then
		fn=os.toWindowsFileName(fn)
	end
	local fout, msg=io.open(fn, "r")
	if fout==nil then
		print(msg)
		return
	end

	contents=fout:read("*a")
	fout:close()
	return contents
end

function util.iterateFile(fn, printFunc)
	printFunc=printFunc or
	{
		iterate=function (self,lineno, line) 
			print(lineno, line)
		end
	}
	local fin, msg=io.open(fn, "r")
	if fin==nil then
		print(msg)
		return
	end
	local ln=1
	--local c=0
	--local lastFn, lastLn
	for line in fin:lines() do
		printFunc:iterate(ln,line)
		ln=ln+1
	end
	fin:close()
end


function util.writeFile(fn, contents)
	if os.isWindows() then
		fn=os.toWindowsFileName(fn)
	end
	local fout, msg=io.open(fn, "w")
	if fout==nil then
		print(msg)
		return msg
	end
	if type(contents)=='string' then
		fout:write(contents)
	else
		fout:write(table.tostring2(contents))
	end
	fout:close()
end
function util.appendFile(fn, arg)
	local fout, msg=io.open(fn, "a")
	if fout==nil then
		print(msg)
		return
	end
	fout:write(arg)
	fout:close()   
end

util.outputToFileShort=util.appendFile
function util.mergeStringShort(arg)
	local out=""
	for i,v in ipairs(arg) do
		local t=type(v)
		if t=='number' then
			out=out.."\t"..string.format("%.2f",v)
		elseif t~="string" then
			out=out.."\t"..tostring(v)
		else
			out=out.."\t"..v
		end
	end
	return out
end

function util.mergeString(arg)
	local out=""
	for i,v in ipairs(arg) do
		if type(t)~="string" then
			out=out.."\t"..tostring(v)
		else
			out=out.."\t"..v
		end
	end
	return out
end

function string.lines(str)
	assert(type(str)=='string')
	local t = {}
	local function helper(line) table.insert(t, line) return "" end
	helper((str:gsub("(.-)\r?\n", helper)))
	return t
end

function string.tokenize(str, pattern)
	local t = {}
	local function helper(line) table.insert(t, line) return "" end
	helper((str:gsub("(.-)"..pattern, helper)))
	return t
end

function string.tokenize2(str, pattern, sep)
	local t = {}
	local function helper(line) if line~="" then table.insert(t, line) end table.insert(t, sep) return "" end
	helper((str:gsub("(.-)"..pattern, helper)))
	table.remove(t, #t)
	return t
end

function string.trimLeft(str)
	a=string.find(str, '[^%s]')
	if a==nil then
		return ""
	end
	return string.sub(str, a)
end
function string.trimRight(str)
	--a=string.find(str, '[%s$]')-- doesn't work
	a=string.find(str, '[%s]',#str)
	if a==nil then return str end
	return string.trimRight(string.sub(str,1,a-1))
end

function deepCopyTable(t)
	assert(type(t)=="table", "You must specify a table to copy")

	local result={}
	for k, v in pairs(t) do
		if type(v)=="table" then
			result[k]=deepCopyTable(v)
		elseif type(v)=="userdata" then
			if v.copy then
				result[k]=v:copy()
			else
				print('Cannot copy '..k)
			end
		else
			result[k]=v
		end
	end

	-- copy the metatable, if there is one
	return setmetatable(result, getmetatable(t))
end

function util.copy(b) -- deep copy
	if type(b)=='table' then
		local result={}
		for k,v in pairs(b) do
			result[k]=util.copy(v)
		end
		-- copy the metatable, if there is one
		return setmetatable(result, getmetatable(b))
	elseif type(b)=='userdata' then
		return b:copy()
	else
		return b
	end
end


function shallowCopyTable(t)
	assert(type(t)=="table", "You must specify a table to copy")

	local result={}

	for k, v in pairs(t) do
		result[k]=v
	end

	-- copy the metatable, if there is one
	return setmetatable(result, getmetatable(t))
end

function table.count(a)
	local count=0
	for k,v in pairs(a) do
		count=count+1
	end
	return count
end

-- t1 and t2 will be shallow copied. you can deepCopy using deepCopy(table.merge(...))
function table.merge(t1, t2)
	local result={}
	assert(type(t1)=='table' and type(t2)=='table')
	for k,v in pairs(t1) do
		result[k]=v
	end
	for k,v in pairs(t2) do
		if result[k] and type(result[k])=='table' and type(v)=='table' then
			result[k]=table.merge(result[k],v)
		else
			result[k]=v
		end
	end
	return result
end

-- note that t2 will be deep copied because it seems to be more useful (and safe)
function table.mergeInPlace(t1,t2, overwrite) -- t1=merge(t1,t2)
	for k,v in pairs(t2) do
		if overwrite or t1[k]==nil then
			t1[k]=util.copy(v)
		end
	end	
end



function os.print(t)
   if type(t)=="table" then
      printTable(t)
   else
      dbg.print(t)
   end
end

function dbg.print(...)
	local arr={...}
	for k,v in ipairs(arr) do
		if type(v)=='userdata' then
			if getmetatable(v).__luabind_class then
				local info=class_info(v)
				if info.methods.__tostring then
					print(v)
				else
					print('userdata which has no __tostring implemented:')
					util.printInfo(v)
				end
			else
				if getmetatable(v).__tostring then
					print(v)
				else
					print('userdata which has no __tostring implemented:')
					printTable(getmetatable(v))
				end
			end
		else
			print(v)
		end
	end
end


function os.createDir(path)

   if os.isUnix() then
      os.execute('mkdir -p "'..path..'"')
   else
      os.execute("md "..os.toWindowsFileName(path))
   end
end

function os.rename(name1, name2)

   if os.isUnix() then
      os.execute('mv "'..name1..'" "'..name2..'"')
   else
      local cmd="move "..os.toWindowsFileName(name1).." "..os.toWindowsFileName(name2)
      print(cmd)
      os.execute(cmd)
   end
end

function os.deleteFiles(mask)

   if os.isUnix() then
      os.execute("rm "..mask)
   else
      os.execute("del "..os.toWindowsFileName(mask))
   end
end

function os.parentDir(currDir)
   return os.rightTokenize(os.fromWindowsFileName(currDir), "/")
end

function os.relativeToAbsolutePath(folder,currDir)

	if string.sub(folder, 1,1)~="/" then
		currDir=currDir or os.currentDirectory()
		while(string.sub(folder,1,3)=="../") do
			currDir=os.parentDir(currDir)
			folder=string.sub(folder,4)
		end
		while(string.sub(folder,1,2)=="./") do
			folder=string.sub(folder,3)
		end
		if folder=="" then
			folder=currDir
		else
			folder=currDir.."/"..folder
		end
	end
	return folder
end
function os.absoluteToRelativePath(folder, currDir) -- param1: folder or file name
	if os.isWindows() then
		folder=os.fromWindowsFileName(folder)
	end
	if(string.sub(folder,1,1)~="/") then return folder end
	currDir=currDir or os.currentDirectory()
	local n_ddot=0
	while string.sub(folder,1,#currDir)~=currDir and currDir~="" do
		currDir=os.parentDir(currDir)
		n_ddot=n_ddot+1
	end
	local str=""
	for i=1,n_ddot do
		str=str.."../"
	end
	--print(n_ddot, currDir)
	str=str..string.sub(folder,#currDir+2)
	return string.trimSpaces(str)
end

function os.currentDirectory()
	if os.isUnix() then
		return os.capture('pwd')
	else
		return os.fromWindowsFileName(os.capture('cd'))
	end
end
function os.copyFile(mask, mask2)

   if os.isUnix() or os.isMsysgit() then
	   if mask2 then
		   os.execute('cp "'..mask..'" "'..mask2..'"')
	   else
		   os.execute("cp "..mask)
	   end
   else
	   if mask2 then
		   print('copy "'..      os.toWindowsFileName(mask)..'" "'..os.toWindowsFileName(mask2)..'"')
		   os.execute('copy "'..      os.toWindowsFileName(mask)..'" "'..os.toWindowsFileName(mask2)..'"')
	   else
		   print("copy "..      os.toWindowsFileName(mask))
		   os.execute("copy "..      os.toWindowsFileName(mask))
	   end
   end
end

-- copy folder a to folder b (assuming folder b doesn't exists yet.)
-- otherwise, behaviors are undefined.
-- os.copyResource("../a", "../b", {'%.txt$', '%.lua$'}) 
function os.copyRecursive(srcFolder, destFolder, acceptedExt)
	if string.sub(destFolder, -1,-1)=="/" then
		destFolder=string.sub(destFolder, 1, -2)
	end

	acceptedExt=acceptedExt or os.globParam.acceptedExt
	local backup=os.globParam.acceptedExt

	os.createDir(destFolder)
	-- first copy files directly in the source folder
	if true then
		os.globParam.acceptedExt=acceptedExt
		local files=os.glob(srcFolder.."/*")
		os.globParam.acceptedExt=backup

		for i,f in ipairs(files) do
			os.copyFiles(f, destFolder.."/"..string.sub(f, #srcFolder+2))
		end
	end

	-- recursive copy subfolders
	local folders=os.globFolders(srcFolder)
	
	for i, f in ipairs(folders) do
		os.copyRecursive(srcFolder.."/"..f, destFolder.."/"..f,  acceptedExt)
	end
end
function os.copyFiles(src, dest, ext) -- copy source files to destination folder and optinally change file extensions.
   if os.isUnix() then
      os.execute('cp "'..src..'" "'..dest..'"')
   else
      local cmd="copy "..os.toWindowsFileName(src).." "..os.toWindowsFileName(dest)
      print(cmd)
      os.execute(cmd)
   end

   if ext then
      local files=os.glob(dest.."/*"..ext[1])

      for i,file in ipairs(files) do
	 if string.find(file, ext[1])~=string.len(file) then
	    os.rename(file,  string.gsub(file, ext[1], ext[2]))
	 end
      end
   end
end

function os._globWin32(attr, mask, ignorepattern)
	local cmd="dir /b/a:"..attr..' "'..os.toWindowsFileName(mask)..'" 2>nul'
	local cap=os.capture(cmd, true)
	local tbl=string.tokenize(cap, "\n")
	tbl[table.getn(tbl)]=nil
	local files={}
	local c=1
	local prefix=""
	--if string.find(mask, "/") then
		--prefix=os.parentDir(mask).."/"
	--else
		--prefix=""
	--end
	for i,fn in ipairs(tbl) do
		if string.sub(fn, string.len(fn))~="~" then
			if not (ignorepattern and string.isMatched(fn, ignorepattern)) then
				files[c]=prefix..fn 
				c=c+1
			end
		end
	end
	return files 
end
os.globParam={}
os.globParam.ignorePath={'^%.', '^CVS'}
os.globParam.acceptedExt={'%.txt$','%.pdf$', '%.inl$', '%.lua$', '%.cpp$', '%.c$', '%.cxx$', '%.h$', '%.hpp$', '%.py$','%.cc$'}

function os.globFolders(path) -- os.globFolders('..') list all folders in the .. folder
	if path==nil or path=="." then
		path=""
	end
	
	if os.isUnix() then
		local path='"'..path..'"'	
		if path=='""' then path="" end
		local tbl=string.tokenize(os.capture('ls -1 -p '..path.." 2> /dev/null",true), "\n")
		local tbl2=array:new()
		local ignorepath=os.globParam.ignorePath
		for i,v in ipairs(tbl) do
			if string.sub(v,-1)=="/" then
				local fdn=string.sub(v, 1,-2)
				if not string.isMatched(fdn, ignorepath) then
					tbl2:pushBack(fdn) 
				end
			end
		end
		return tbl2
	else
		return os._globWin32('D', path, os.globParam.ignorePath)
	end
end

function os._processMask(mask)
	mask=string.gsub(mask, "#", "*")
	local folder, lmask=os.rightTokenize(mask, '/',true)
	local wildcardCheckPass=true
	if not string.find(lmask, '[?*]') then -- wild card
		folder=folder..lmask
		lmask="*"
	end
	if string.sub(folder, -1)~="/" and folder~="" then
		folder=folder.."/"
	end
	return folder,lmask 
end

function os.findgrep(mask, bRecurse, pattern)
	local printFunc={ 
		iterate=function (self, v)
			util.grepFile(v, pattern)
		end
	}	
	os.find(mask, bRecurse, true, printFunc)
end
function os.find(mask, bRecurse, nomessage, printFunc) 
	local printFunc=printFunc or { iterate=function (self, v) print(v) end}
	--mask=string.gsub(mask, "#", "*")
	--mask=string.gsub(mask, "%%", "*")
	if string.find(mask, "[#%%?%*]")==nil then
		-- check if this is a file not a folder
		if os.isFileExist(mask) then
			printFunc:iterate(mask)
			return 
		else
			fn,path=os.processFileName(mask)-- fullpath
			mask=path.."/*"..fn -- try full search
		end
	end
	if not nomessage then
		io.write('globbing '..string.sub(mask,-30)..'                               \r')
	end
	if bRecurse==nil then bRecurse=false end

	if os.isUnix() then 
		local folder, lmask=os.rightTokenize(mask, '/')
		if lmask=="*" then
			mask=folder
			folder, lmask=os.rightTokenize(mask, '/')
		end
		local containsRelPath=false
		if string.find(lmask, '[?*]') then -- wild card
			containsRelPath=true
		end
		local cmd='ls -1 -p '..mask..' 2>/dev/null'
		local tbl=string.tokenize(os.capture(cmd,true), "\n")
		local lenfolder=string.len(folder)
		--print(cmd,mask,#tbl,lenfolder)
		if lenfolder==0 then lenfolder=-1 end
		local acceptedExt=deepCopyTable(os.globParam.acceptedExt)

		if string.find(mask,"%*%.") then
			local idx=string.find(mask,"%*%.")+2
			acceptedExt[#acceptedExt+1]="%."..string.sub(mask,idx)..'$'
			--print(acceptedExt[#acceptedExt])
		end

		for i=1, table.getn(tbl)-1 do
			local v=tbl[i]
			if string.sub(v,-1)~="/" and string.isMatched(v, acceptedExt) then
				if containsRelPath then
					printFunc:iterate(v)
				else
					if string.sub(mask,-1)=="/" then
						printFunc:iterate(mask..v)
					else
						printFunc:iterate(mask.."/"..v)
					end
				end
			end
		end
	else
		local folder, lmask=os._processMask(mask)
		local out=os._globWin32("-d", folder..lmask)
		local acceptedExt=deepCopyTable(os.globParam.acceptedExt)

		if string.find(mask,"%*%.") then
			local idx=string.find(mask,"%*%.")+2
			acceptedExt[#acceptedExt+1]="%."..string.sub(mask,idx)..'$'
			--print(acceptedExt[#acceptedExt])
		end
		for i=1, table.getn(out) do
			if string.isMatched(out[i], acceptedExt) then
				printFunc:iterate(folder..out[i])
			end
		end
	end
	local verbose=false
	if bRecurse then
		local folder, lmask=os._processMask(mask)
		if verbose then print(folder, lmask) end
		local subfolders=os.globFolders(folder)
		if verbose then printTable(subfolders) end
		for i=1, table.getn(subfolders) do
			local v=subfolders[i]
			os.find(folder..v..'/'..lmask, true, nomessage, printFunc)
		end
	end
	if not nomessage then
		io.write('                                                                   \r')
	end
end
function os.glob(mask, bRecurse, nomessage) -- you can use # or % instead of *. e.g. os.glob('#.jpg')

	local tbl2=array:new()
	function tbl2:iterate(v)
		--print(v)
		self:pushBack(v)
	end
	os.find(mask, bRecurse, nomessage, tbl2)
	return tbl2
end

function os.home_path()
	if os.isUnix() then
		--return os.capture("echo ~")
		return os.getenv('HOME')
	else
		return os.capture("echo %HOMEDRIVE%")..os.capture("echo %HOMEPATH%")
	end
end
-- returns filename, path
function os.processFileName(target)-- fullpath
	local target=os.fromWindowsFileName(target)
	local lastSep
	local newSep=0
	local count=0
	repeat lastSep=newSep
		newSep=string.find(target, "/", lastSep+1) 	    
		count=count+1
	until newSep==nil 

	local path=string.sub(target, 0, lastSep-1)

	local filename
	if lastSep==0 then filename=string.sub(target,lastSep) path='' else filename=string.sub(target, lastSep+1) end

	return filename, path
end
function os.filename(target)
	local f=os.processFileName(target)
	return f
end

function os.isFileExist(fn)
	local f=io.open(fn,'r')
	if f==nil then return false end
	f:close()
	return true
end
function os.createBatchFile(fn, list, echoOff)
	local fout, msg=io.open(fn, "w")
	if fout==nil then print(msg) end

	if os.isWindows() then
		if echoOff then
			fout:write("@echo off\necho off\n")
		end
		fout:write("setlocal\n")
	end
	for i,c in ipairs(list) do
		fout:write(c.."\n")
	end
	fout:close()
end

function os.execute2(...) -- excute multiple serial operations
	local list={...}
	if not math.seeded then
		math.randomseed(os.time())
		math.seeded=true
	end
	if os.isUnix() then
		if #list<3 then
			local cmd=""
			for i,c in ipairs(list) do
				cmd=cmd..";"..c
			end
			--print(string.sub(cmd,2))
			os.execute(string.sub(cmd, 2))
		else
			local fn='temp/_temp'..tostring(math.random(1,10000))
			os.createBatchFile(fn, list)
			os.execute("sh "..fn)
			os.deleteFiles(fn)
		end
	else
		os.createBatchFile("_temp.bat",list,true)
		--      os.execute("cat _temp.bat")
		os.execute("_temp.bat")	
	end
end

function os.pexecute(...) -- excute multiple serial operations
	if os.isUnix() then
		os.execute2(...)
	else
		local list={...}
		os.createBatchFile("_temp.bat",list)      
		os.execute("start _temp.bat")	
	end      
end

-- escape so that it can be used in double quotes
function os.shellEscape(str)
	if os.isUnix() then
		str=string.gsub(str, '\\', '\\\\')
		str=string.gsub(str, '"', '\\"')
		str=string.gsub(str, '%%', '\\%%')
	else
		str=string.gsub(str, '\\', '\\\\')
		str=string.gsub(str, '"', '^"')
		str=string.gsub(str, '%$', '\$')
	end
	return str
end

function os.luaExecute(str, printCmd)
	local luaExecute
	local gotoRoot
	local endMark
	local packagepath=os.shellEscape('package.path="./OgreFltk/Resource/scripts/ui/?.lua;./OgreFltk/work/?.lua"')
	if os.isUnix() then
		luaExecute="lua -e \""..packagepath.."dofile('OgreFltk/work/mylib.lua');"
		gotoRoot="cd ../.."
		endMark="\""
	else
		luaExecute="OgreFltk\\work\\lua -e dofile('OgreFltk/work/mylib.lua');"
		gotoRoot="cd ..\\.."
		endMark=""
	end
	str=os.shellEscape(str)
	if printCmd then print(luaExecute..str..endMark) end
	os.execute2(gotoRoot, luaExecute..str..endMark)
end

--use util.grepFile(fn, pattern)
function util.grepFile(fn, pattern, prefix,useLuaPattern, printFunc)
	printFunc=printFunc or 
	{ 
		iterate=function(self,fn,ln,idx,line)
			print(fn..":"..ln..":"..string.trimLeft(line))
		end
	}
	prefix=prefix or ""
	pattern=string.lower(pattern)
	local fin, msg=io.open(fn, "r")
	if fin==nil then
		print(msg)
		return
	end
	local ln=1
	--local c=0
	--local lastFn, lastLn
	for line in fin:lines() do
		local lline=string.lower(line)
		local res, idx
		if useLuaPattern then
			res,idx=pcall(string.find, lline, pattern)
		else
			res,idx=pcall(string.find, lline, pattern, nil,true)
		end
		if res and idx then 
			--				print(prefix..fn..":"..ln..":"..idx..":"..string.trimLeft(line))
			printFunc:iterate(prefix..fn,ln,idx,line)
			--c=c+1
			--lastFn=prefix..fn
			--lastLn=ln
		end
		ln=ln+1
	end
	fin:close()
	--if c==1 then
		--os.vi_line(lastFn, lastLn)
	--end
end
function os.open(t)
	if os.isUnix() then
		os.execute('gnome-open '..t)
	else
		os.execute('start cmd/c '..t)
	end
end
