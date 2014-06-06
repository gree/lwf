def parse_params(args)
	result = []
	args.split(',').each do | arg |
		result << arg.split(" ")
	end
	result
end

def check_statement(type, i)
	case type
	when "float","int"
		return "Lua.lua_isnumber(L, #{i})==0"
	when "string"
		return "Lua.lua_isstring(L,#{i})==0"
	when "bool"
		return "!Lua.lua_isboolean(L,#{i})"
	else
		type_nodot = type.gsub(".","_")
		return "Luna.get_uniqueid(L,#{i})!= LunaTraits_#{type_nodot}.uniqueID"
	end
end

def check_right_statement(type, i)
	case type
	when "float","int"
		return "Lua.lua_isnumber(L, #{i})==1"
	when "string"
		return "Lua.lua_isstring(L,#{i})==1"
	when "bool"
		return "Lua.lua_isboolean(L,#{i})"
	else
		type_nodot = type.gsub(".","_")
		return "Luna.get_uniqueid(L,#{i})== LunaTraits_#{type_nodot}.uniqueID"
	end
end

def conv_func(type, i)
	case type
	when "float"
		return "(float)Lua.lua_tonumber(L,#{i})"
	when "int"
		return "(int)Lua.lua_tonumber(L,#{i})"
	when "bool"
		return "Lua.lua_toboolean(L,#{i}) != 0"
	when "string"
		return "Lua.lua_tostring(L,#{i}).ToString()"
	else
		type_nodot = type.gsub(".","_")
		"Luna_#{type_nodot}.check(L,#{i})"
	end
end

def push_func(type)
	case type
	when "float","int"
		return "Lua.lua_pushnumber(L, ret)"
	when "string"
		return "Lua.lua_pushstring(L, ret)"
	when "bool"
		return "Lua.lua_pushboolean(L, ret?1:0)"
	else
		type_nodot = type.gsub(".","_")
		return "Luna_#{type_nodot}.push(L,ret,true,\"#{type_nodot}\")"
	end
end

def typecheck_condition_for_ctor(ctor)
	params = parse_params(ctor.gsub(/\(|\)/,""))
	result = "Lua.lua_gettop(L)!= #{params.size}"
	params.each_with_index do | p,i |
		type, name = p
		result += "\n" + " "*4*3 + "|| " + check_statement(type, i+1)
	end
	result
end

def typecheck_condition_for_getter
	"Lua.lua_gettop(L)!= 1" + "\n" + " "*4*3 + "|| Luna.get_uniqueid(L,1)!=__UNIQUE_ID__"
end

def typecheck_condition_for_setter(property_type)
	"Lua.lua_gettop(L)!= 2" +
	"\n" + " "*4*3 + "|| Luna.get_uniqueid(L,1)!=__UNIQUE_ID__ " +
	"\n" + " "*4*3 + "|| " + check_statement(property_type, 2)
end

def typecheck_condition_for_not_member_function(params)
	s = "Lua.lua_gettop(L)!=#{params.length+1}" +
	"\n" + " "*4*3 + "|| Luna.get_uniqueid(L,1)!=__UNIQUE_ID__ "
	params.each_with_index do | p, i |
		s += "\n" + " "*4*3 + "|| " + check_statement(p[0], i+2)
	end
	s
end

def typecheck_condition_for_member_function(params)
	s = "Lua.lua_gettop(L)==#{params.length+1}" +
	"\n" + " "*4*3 + "|| Luna.get_uniqueid(L,1)==__UNIQUE_ID__ "
	params.each_with_index do | p, i |
		s += "\n" + " "*4*3 + "|| " + check_right_statement(p[0], i+2)
	end
	s
end

def typecheck_condition_for_static_member_function(params)
	s = "Lua.lua_gettop(L)!=#{params.length}" +
	"\n" + " "*4*3 + "|| Luna.get_uniqueid(L,1)!=__UNIQUE_ID__ "
	params.each_with_index do | p, i |
		next if i == 0
		s += "\n" + " "*4*3 + "|| " + check_statement(p[0], i+1)
	end
	s
end

def parse_static_member_functions(src)
	return [] if src.nil?
	result = []
	src.lines do | line |
		if md = line.match(/(.*)\s+([^\s]+)\((.*)\);/)
			return_type = md[1].split(" ").last
			name = md[2]
			params = parse_params(md[3])
			result << {"name" => name, "return_type" => return_type, "params" => params}
		end
	end
	result
end

def parse_member_functions(src)
	return [] if src.nil?
	result = []
	src.lines do | line |
		if md = line.match(/(.*)\s+([^\s]+)\((.*)\).*@\s*(.+)/)
			return_type = md[1].split(" ").last
			name = md[2]
			params = parse_params(md[3])
			alias_name = md[4]
			result << {"name" => name, "return_type" => return_type, "params" => params, "alias_name" => alias_name}
		end
	end
	result
end

def output_name_no_dot
	@name.gsub(".","_")
end

def output_registries
	registries = []
	@member_functions.each do | f |
		name = f["alias_name"]
		registries << [name, "_bind_" + name] unless registries.any? { |a| a[0] == name}
	end

	(@getters+@setters).each do | f |
		name = f[1]
		registries << [name, "_bind_" + name] unless registries.any? { |a| a[0] == name}
	end

	@custom_functions_to_register.each do | name |
		registries << [name, name] unless registries.any? { |a| a[0] == name}
	end

	registries.inject("") do | result, a |
		name,func = a
		result + "        new RegType(\"#{name}\", impl_LunaTraits___T_NODOT__.#{func}),\n"
	end
end

def output_init_hashmap
	@getters.inject("") do | result, a |
		result + "        LunaTraits___T_NODOT__.properties[\"#{a[0]}\"]=_bind_#{a[1]};\n"
	end
end

def output_init_write_hashmap
	@setters.inject("") do | result, a |
		result +"         LunaTraits___T_NODOT__.write_properties[\"#{a[0]}\"]=_bind_#{a[1]};\n"
	end
end

def output_bind_ctors
	result = ""
	@ctors.each_with_index do | p, i |
		result +=<<EOS
	public static __T__ _bind_ctor_overload_#{i+1}(Lua.lua_State L)
	{
EOS
		params = parse_params(p.gsub(/\(|\)/,""))
		params.each_with_index do | p,i |
			type, name = p
			result += <<EOS
		#{type} #{name}=#{conv_func(type, i+1)};
EOS
		end

		result +=<<EOS
		return new __T__(#{params.map {|type, name| name}.join(", ")});
	}
EOS
	end
	if @ctors.length > 0
		result +=<<EOS
	public static __T__ _bind_ctor(Lua.lua_State L)
	{
EOS

		@ctors.each_with_index do | ctor, i |
		result +=<<EOS
		if (#{typecheck_condition_for_ctor(ctor)}) return _bind_ctor_overload_#{i+1}(L);
EOS
		end

		result +=<<EOS
		Lua.luaL_error(L, "ctor ( cannot find overloads:)");

		return null;
	}
EOS
	end
	result
end

def output_property_accesors
	result = ""
	@property_types.each do | name ,type |
		getter = @property_getters.find { |a| a[0] == name }[1]
		setter = @property_setters.find { |a| a[0] == name }[1]
		result +=<<EOS
	public static #{type} #{getter}(__T__ a) { return a.#{name}; }
	public static void #{setter}(__T__ a, #{type} b) { a.#{name}=b; }
EOS
	end
	result
end

def output_bind_property_methods
	result = ""
	@property_types.each do | name ,type |
		getter = @property_getters.find { |a| a[0] == name }[1]
		setter = @property_setters.find { |a| a[0] == name }[1]
		result +=<<EOS
	public static int _bind_#{getter}(Lua.lua_State L)
	{
		if (#{typecheck_condition_for_getter})
		{
			Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:#{getter}(__T__ a)");
		}

		__T__ a=Luna___T_NODOT__.check(L,1);
		try {
			#{type} ret=#{getter}(a);
			#{push_func(type)};
		} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
		return 1;
	}

	public static int _bind_#{setter}(Lua.lua_State L)
	{
		if (#{typecheck_condition_for_setter(type)})
		{
			Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:#{setter}(__T__ a, #{type} b)");
		}

		__T__ a=Luna___T_NODOT__.check(L,1);
		#{type} b=(#{type})#{conv_func(type,2)};
		try {
			#{setter}(a, b);
		} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
		return 0;
	}

EOS
	end
	result
end

def output_bind_member_functions
	overloads = Hash.new(0)
	@member_functions.each do | f |
		overloads[f["alias_name"]] += 1
	end

	result = ""
	@member_functions.each do | f |
		name = f["name"]
		return_type = f["return_type"]
		params = f["params"]
		alias_name = f["alias_name"]

		typecheck = ""
		if overloads[alias_name] > 1
			id = 1
			@member_functions.each do | t |
				next if alias_name != t["alias_name"]
				break if t == f
				id += 1
			end
			alias_name += "_overload_" + id.to_s
		else
			typecheck =<<EOS
	if (#{typecheck_condition_for_not_member_function(params)}) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:#{alias_name}(__T__ self)"); }
EOS
		end
		result +=<<EOS
  public static int _bind_#{alias_name}(Lua.lua_State L)
  {
#{typecheck}
	__T__ self=Luna___T_NODOT__.check(L,1);
EOS
		params.each_with_index do | param, i |
			p_type, p_name = param
			result +=<<EOS
		#{p_type} #{p_name}=#{conv_func(p_type,i+2)};
EOS
		end

		param_names = params.map { | a| a[1]}.join(", ")
		if return_type == "void"
			result +=<<EOS
	try {
		self.#{name}(#{param_names});
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 0;
  }
EOS
		else
			result +=<<EOS
	try {
		#{return_type} ret=self.#{name}(#{param_names});
		#{push_func(return_type)};
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 1;
  }
EOS
		end
	end

	overloads.each do | alias_name, num |
		next if num < 2
		result +=<<EOS
  public static int _bind_#{alias_name}(Lua.lua_State L)
  {
EOS
		id = 1
		@member_functions.each do | f |
			next if f["alias_name"] != alias_name
			params = f["params"]
			result +=<<EOS
	if (#{typecheck_condition_for_member_function(params)}) return _bind_#{alias_name}_overload_#{id}(L);
EOS
			id += 1
		end
		result +=<<EOS
	Lua.luaL_error(L, "#{alias_name} cannot find overloads.");

	return 0;
  }
EOS
	end
	result
end

def output_bind_static_member_functions
	result = ""
	@static_member_functions.each do | f |
		name = f["name"]
		return_type = f["return_type"]
		params = f["params"]

		result +=<<EOS
  public static int _bind_#{name}(Lua.lua_State L)
  {
	if (#{typecheck_condition_for_static_member_function(params)}) { Luna.printStack(L); Lua.luaL_error(L, "luna typecheck failed:#{name}(__T__ self ...)"); }
EOS
		params.each_with_index do | param, i |
			p_type, p_name = param
			result +=<<EOS
		#{p_type} #{p_name}=#{conv_func(p_type,i+1)};
EOS
		end

		param_names = params.map { | a| a[1]}.join(", ")
		if return_type == "void"
			result +=<<EOS
	try {
		#{name}(#{param_names});
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 0;
  }
EOS
		else
			result +=<<EOS
	try {
		#{return_type} ret=#{name}(#{param_names});
		#{push_func(return_type)};
	} catch(Exception e) { Lua.luaL_error( L,new Lua.CharPtr(e.ToString())); }
	return 1;
  }
EOS
		end
	end
	result
end

def output_unique_id(str)
	h = 0
	str.bytes.each_with_index do | b,i |
		h = 31*h+b
		h = h % (100000000+i)
	end
	h.to_s
end

input_file = ARGV[0]
prefix = ARGV[1]
input = ""
File.open(input_file) { | f | input = eval(f.read) }

template = nil
File.open("luabinding.cs.template") { | f | template = f.read }

input[:classes].each do | c |
	@property_types = (c[:properties] || []).inject({}) do | result, p |
		type, name = p.split
		result[name] = type
		result
	end

	@property_getters = @property_types.keys.map do | prop_name |
		[prop_name, "_property_get_" + prop_name]
	end

	@property_setters = @property_types.keys.map do | prop_name |
		[prop_name, "_property_set_" + prop_name]
	end

	@getters = @property_getters + (c[:readProperties] || [])
	@setters = @property_setters + (c[:writeProperties] || [])

	@ctors = c[:ctors] || {}
	@member_functions = parse_member_functions(c[:memberFunctions])
	@static_member_functions = parse_static_member_functions(c[:staticMemberFunctions])
	@custom_functions_to_register = (c[:customFunctionsToRegister] || [])
	@name = c[:name]

	unique_id = output_unique_id(@name)
	wrapper_code = c[:wrapperCode] || ""
	custom_index = c[:customIndex] || ""
	custom_new_index = c[:customNewIndex] || ""

	code =
	[
	["__TRAITS_BIND_METHODS__", output_bind_ctors + output_property_accesors + output_bind_property_methods + output_bind_member_functions + output_bind_static_member_functions],
	["__REGISTORIES__", output_registries],
	["__WRAPPER_CODE__", wrapper_code],
	["__CUSTOM_INDEX__", custom_index],
	["__CUSTOM_NEWINDEX__", custom_new_index],
	["__INIT_HASHMAP__", output_init_hashmap],
	["__INIT_WRITEHASHMAP__",output_init_write_hashmap],
	["__UNIQUE_ID__", unique_id],
	["__T_NODOT__",output_name_no_dot],
	["__T__", @name],
	].inject(template) do | result, a |
		result.gsub(a[0], a[1])
	end
	File.open("#{prefix}_luabinding_#{output_name_no_dot.downcase}.cs","w") { | f | f.write(code) }
end

