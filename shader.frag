#version 440

out vec4 outputColor;

in vec2 texCoord;
in vec3 Normal;
in vec3 FragPos;


uniform vec3 lightPos;
uniform vec3 viewPos;


struct Material {
    sampler2D diffuse;
    sampler2D texture_diffuse1;
    sampler2D texture_diffuse2;
    sampler2D texture_diffuse3;
    sampler2D texture_diffuse4;
    sampler2D texture_diffuse5;
    sampler2D texture_diffuse6;
    sampler2D texture_diffuse7;
    sampler2D texture_diffuse8;
    sampler2D texture_diffuse9;
    sampler2D texture_diffuse10;
    sampler2D texture_diffuse11;
    sampler2D texture_diffuse12;
    sampler2D texture_diffuse13;
    sampler2D texture_diffuse14;
    sampler2D texture_diffuse15;
    sampler2D texture_diffuse16;
    sampler2D texture_diffuse17;
    sampler2D texture_diffuse18;

    sampler2D texture_specular1;
    sampler2D texture_specular2;
    sampler2D texture_specular3;
    sampler2D texture_specular4;
    sampler2D texture_specular5;

    sampler2D specular;
    sampler2D emission;
    float     shininess;
}; 

struct PointLight {
    vec3 position;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;

    float constant;
    float linear;
    float quadratic;
};

struct DirectionalLight{
    vec3 direction;

    vec3 diffuse;
    vec3 specular;    
};

struct Spotlight{
    vec3 position;
    vec3 direction;
    float inner_corner;
    float outer_corner;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

uniform Material material;

#define NR_POINT_LIGHTS 4  
uniform PointLight pointLights[NR_POINT_LIGHTS];

uniform DirectionalLight dirLight;
uniform Spotlight spotlight;


vec3 GetMixedDiffuse(Material material, vec2 texCoord)
{
    vec4 out_color;
    out_color = mix(texture(material.texture_diffuse1, texCoord), texture(material.texture_diffuse2, texCoord), 0.5f);
    out_color += mix(out_color, texture(material.texture_diffuse3, texCoord), 0.5f);
    out_color += mix(out_color, texture(material.texture_diffuse4, texCoord), 0.5f);
    out_color += mix(out_color, texture(material.texture_diffuse5, texCoord), 0.5f);
    out_color += mix(out_color, texture(material.texture_diffuse6, texCoord), 0.5f);
    out_color += mix(out_color, texture(material.texture_diffuse7, texCoord), 0.5f);
    out_color += mix(out_color, texture(material.texture_diffuse8, texCoord), 0.5f);
    out_color += mix(out_color, texture(material.texture_diffuse9, texCoord), 0.5f);
    out_color += mix(out_color, texture(material.texture_diffuse10, texCoord), 0.5f);

    return out_color.rgb;
}

vec3 GetMixedSpecular(Material material, vec2 texCoord)
{
    vec4 out_color;
    out_color = mix(texture(material.texture_specular1, texCoord), texture(material.texture_specular2, texCoord), 0.5f);
    out_color += mix(out_color, texture(material.texture_specular3, texCoord), 0.5f);
    out_color += mix(out_color, texture(material.texture_specular4, texCoord), 0.5f);
    out_color += mix(out_color, texture(material.texture_specular5, texCoord), 0.5f);

    return out_color.rgb;
}



vec3 directionalLight(DirectionalLight dirLight, Material material, vec3 norm, vec3 viewDir)
{

    //diffuse
    vec3 lightDir = normalize(-dirLight.direction);
    float diff = max(dot(norm, lightDir), 0.0);
    vec3 out_color = dirLight.diffuse * diff * GetMixedDiffuse(material, texCoord);

    //specular
    vec3 reflectDir = reflect(-lightDir, norm);
    float spec = pow(max(dot(reflectDir, viewDir), 0.0f), material.shininess);
    out_color += dirLight.specular * spec *GetMixedSpecular(material, texCoord).rgb;

    return out_color;
}

vec3 pointLight(PointLight light, Material material, vec3 norm, vec3 viewDir, vec3 FragPos)
{
    float distance = length(FragPos - light.position);
    float attenuation = 1.0f / (light.constant + light.linear * distance + light.quadratic * (distance * distance));

    //diffuse
    vec3 lightDir = normalize(light.position - FragPos);
    float diff = max(dot(norm, lightDir), 0.0);
    vec3 out_color = light.diffuse * diff *GetMixedDiffuse(material, texCoord) * attenuation;

    //specular
    float specularStrength = 0.5f;

    vec3 reflectDir = reflect(-lightDir, norm);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    out_color += vec3(GetMixedSpecular(material, texCoord)) * spec * light.specular  * attenuation;

    return out_color; 
}

vec3 calcSpotlight(Spotlight light, Material material, vec3 norm, vec3 viewDir, vec3 FragPos)
{
    vec3 lightDir = normalize(spotlight.position - FragPos);
    float theta = dot(lightDir, normalize(-spotlight.direction));
    float diff = max(dot(norm, lightDir), 0.0);
    float I = (theta - spotlight.outer_corner)/(spotlight.inner_corner - spotlight.outer_corner);
    I = clamp(I, 0.0f, 1.0f);
    vec3 out_color = spotlight.diffuse * diff *GetMixedDiffuse(material, texCoord) * I;

    float specularStrength = 1.0f;
    vec3 reflectDir = reflect(-lightDir, norm);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    out_color += vec3(GetMixedSpecular(material, texCoord)) * spec * spotlight.specular * I;

    return out_color;   
}

void main()
{
    vec3 ambient = vec3(0.0f);
    vec3 diffuse = vec3(0.0f);
    vec3 specular = vec3(0.0f);

    vec3 out_color = vec3(0.0f);
    vec3 viewDir = normalize(viewPos - FragPos);
    vec3 norm = normalize(Normal);




//    out_color += directionalLight(dirLight, material, norm, viewDir);

    for(int i = 0; i < NR_POINT_LIGHTS; i++)
    {
        out_color += pointLight(pointLights[i], material, norm, viewDir, FragPos);
        out_color += pointLights[i].ambient *GetMixedDiffuse(material, texCoord);
    }

    out_color += calcSpotlight(spotlight, material, norm, viewDir, FragPos);



    //out_color += vec3(texture(material.emission, texCoord)).rgb;

    outputColor = vec4(out_color, 1.0f);
}